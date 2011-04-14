using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using Ketchup.Config;
using Ketchup.IO;

namespace Ketchup
{
	public class Node
	{
		private readonly object _sync = new object();
		private readonly ConcurrentQueue<Socket> _socketPool = new ConcurrentQueue<Socket>();
		private readonly ManualResetEvent _handle = new ManualResetEvent(false);
		private int _totalSocketsCreated;
		
		public int Port { get; set; }
		public int CurrentRetryCount { get; set; }
		public bool IsDead { get; set; }
		public string Host { get; set; }
		public DateTime DeadAt { get; set; }
		public DateTime LastConnectionFailure { get; set; }
		
		public string Id
		{
			get { return Host + ":" + Port; }
		}

		public Socket GetSocketFromPool()
		{
			Socket socket;
			if (_socketPool.TryDequeue(out socket)) {
				Debug.WriteLine(Host + " Socket Re-used: " + _totalSocketsCreated);
				return socket;
			}

			var maxPooledSockets = KetchupConfig.Current.MaxPooledSockets;
			lock(_sync) {
				if (_totalSocketsCreated < maxPooledSockets) {
					socket = Connect(CreateSocket());
					_totalSocketsCreated++;
					Debug.WriteLine(Host + " Socket Created: " + _totalSocketsCreated);
					return socket;
				}
			}
			
			_handle.Reset();
			var maxWait = new TimeSpan(0, 0, KetchupConfig.Current.MaxPooledSocketWait);
			Debug.WriteLine(Host + " Wait");
			if (!_handle.WaitOne(maxWait))
				throw new TimeoutException("Ketchup client timed out while waiting for a socket connection from the pool");

			return GetSocketFromPool();
		}

		public void ReleaseSocket(Socket socket) 
		{
			_socketPool.Enqueue(socket);
			_handle.Set();
		}

		public Node()
		{
			IsDead = false;
			CurrentRetryCount = -1;
			DeadAt = DateTime.MinValue;
			LastConnectionFailure = DateTime.MinValue;
		}

		public Node(string host, int port)
			: this()
		{
			Host = host;
			Port = port;
		}

		private Socket Connect(Socket socket)
		{
			if (socket != null && socket.Connected) return socket;
			if (IsDead) throw new Exception("Node is dead");
			if (socket == null) socket = CreateSocket();

			if (string.IsNullOrEmpty(Host)) throw new Exception("Host is null or empty string, cannot connect to socket");
			if (Port == default(int)) throw new Exception("Port is not valid for this Node, cannot connect to socket");

			try
			{
				//if a connection error happened before, recurse until reconnect time has passed
				if (!ReadyToTry()) Connect(socket);

				//the big kahuna
				socket.Connect(Host, Port);

			}
			catch (SocketException ex)
			{
				//at least retry timeouts the specified number of times
				if (ex.SocketErrorCode == SocketError.TimedOut) return HandleTimeout(socket);
			}

			return socket;
		}

		private static Socket CreateSocket()
		{
			var config = KetchupConfig.Current;

			return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
			{
				SendTimeout = (int)config.ConnectionTimeout.TotalMilliseconds,
				ReceiveTimeout = (int)config.ConnectionTimeout.TotalMilliseconds,
				NoDelay = true,
				Blocking = true,
				UseOnlyOverlappedIO = false,
				DontFragment = true,
			};
		}

		private bool ReadyToTry()
		{
			if (LastConnectionFailure == DateTime.MinValue) return true;
			return (DateTime.Now - LastConnectionFailure) >= KetchupConfig.Current.ConnectionRetryDelay;
		}

		private Socket HandleTimeout(Socket socket)
		{
			LastConnectionFailure = DateTime.Now;

			if (CurrentRetryCount++ >= KetchupConfig.Current.ConnectionRetryCount)
			{
				IsDead = true;
				DeadAt = LastConnectionFailure;
				return socket;
			}

			//haven't reached the max retries yet, let's try again until we do
			return Connect(socket);
		}

		public static int GetPort(string endpoint)
		{
			int port;
			var portString = endpoint.Contains(":") ? endpoint.Split(':')[1] : "11211";
			if (!int.TryParse(portString, out port)) throw new ConfigurationErrorsException("The specified port is not a valid int integer");
			return port;
		}
	}
}


