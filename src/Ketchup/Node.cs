using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Net.Sockets;
using Ketchup.Config;
using System.Collections.Generic;
using Ketchup.Protocol.Operations;

namespace Ketchup
{
	public class Node
	{
		private readonly object _sync = new object();
		private readonly Processor _processor;
		private Socket _nodeSocket;

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

		public Socket NodeSocket
		{
			get
			{
				lock (_sync)
				{
					if (_nodeSocket == null)
						_nodeSocket = Connect(CreateSocket());
				}
				return Connect(_nodeSocket);
			}
		}

		public Node()
		{
			IsDead = false;
			CurrentRetryCount = -1;
			DeadAt = DateTime.MinValue;
			LastConnectionFailure = DateTime.MinValue;

			_processor = new Processor(this);
			_processor.Start();
		}

		public Node(string host, int port)
			: this()
		{
			Host = host;
			Port = port;
		}

		public void Request(byte[] packet, Action<byte[], object> process, Action<Exception, object> error, object state)
		{
			var op = new Operation()
			{
				Error = error,
				Process = process,
				Packet = packet,
				State = state,
				Socket = NodeSocket,
			};

			//the operation is picked up by a different thread later which deals with the packets
			_processor.WriteQueue.Enqueue(op);
			//Thread.Sleep(20);
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
				throw;
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
				UseOnlyOverlappedIO = false
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


