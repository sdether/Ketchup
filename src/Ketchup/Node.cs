using System;
using System.Configuration;
using System.Net.Sockets;
using Ketchup.Config;

namespace Ketchup {
	public class Node {

		private readonly byte[] _buffer = new byte[1024];
		private Socket _socket;
		
		public int Port { get; set; }
		public int CurrentRetryCount { get; set; }
		public bool	IsDead { get; set; }
		public string Host { get; set; }
		public DateTime DeadAt { get; set; }
		public DateTime LastConnectionFailure { get; set; }

		public string Id {
			get { return Host + ":" + Port; }
		}

		public Node() {
			IsDead = false;
			CurrentRetryCount = 0;
			DeadAt = DateTime.MinValue;
			LastConnectionFailure = DateTime.MinValue;
		}

		public Node(string host, int port) : this() {
			Host = host;
			Port = port;
		}

		public void Request(byte[] packet, Action<byte[]> callback) {
			var state = new NodeAsyncState(){ Callback = callback, Socket = Connect()};
			state.Socket.BeginSend(packet, 0, packet.Length, SocketFlags.None, SendData, state);
		}

		private void SendData(IAsyncResult asyncResult) {
			var state = (NodeAsyncState)asyncResult.AsyncState;
			var remote = state.Socket;

			remote.EndSend(asyncResult);
			remote.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveData, state);			
		}

		private void ReceiveData(IAsyncResult asyncResult) {
			var state = (NodeAsyncState)asyncResult.AsyncState;
			var remote = state.Socket;

			remote.EndReceive(asyncResult);
			if (state.Callback != null)
				state.Callback(_buffer);
		}

		private Socket Connect() {
			return Connect(_socket);
		}

		private Socket Connect(Socket socket) {
			if (IsDead) 
				throw new Exception("Node is dead");

			if (socket != null && socket.Connected)
				return socket;

			if (string.IsNullOrEmpty(Host))
				throw new Exception("Host is null or empty string, cannot connect to socket");

			if (Port == default(int))
				throw new Exception("Port is not valid for this Node, cannot connect to socket");

			if (socket == null)
				socket = CreateSocket();

			try {
				//if a connection error happened before, recurse until reconnect time has passed
				if(!ReadyToTry()) 
					return Connect(socket);

				//the big kahuna
				socket.Connect(Host, Port);

			} catch (SocketException ex) {

				//at least retry timeouts the specified number of times
				if(ex.SocketErrorCode == SocketError.TimedOut) 
					return HandleTimeout(socket);
				
				throw;
			}

			return socket;
		}

		private static Socket CreateSocket() {
			var config = KetchupConfig.Current;

			return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
				SendTimeout = config.ConnectionTimeout.Milliseconds,
				ReceiveTimeout = config.ConnectionTimeout.Milliseconds,
				NoDelay = true,
				Blocking = true,
				UseOnlyOverlappedIO = false
			};
		}

		private bool ReadyToTry() {
			if(LastConnectionFailure == DateTime.MinValue)
				return true;

			return (DateTime.Now - LastConnectionFailure) >= KetchupConfig.Current.ConnectionRetryDelay;
		}

		private Socket HandleTimeout(Socket socket) {
			LastConnectionFailure = DateTime.Now;
			
			//haven't reached the max retries yet, let's try again until we do
			if (CurrentRetryCount < KetchupConfig.Current.ConnectionRetryCount){
				CurrentRetryCount++;
				return Connect(socket);
			}

			IsDead = true;
			DeadAt = LastConnectionFailure;
			
			return socket;
		}

		public static int GetPort(string endpoint) {
			int port;

			var portString = endpoint.Contains(":") ? endpoint.Split(':')[1] : "11211";
			if (!int.TryParse(portString, out port))
				throw new ConfigurationErrorsException("The specified port is not a valid int integer");

			return port;
		}

		private class NodeAsyncState {
			public Socket Socket { get; set; }
			public Action<byte[]> Callback { get; set; }
		}
	}
}


