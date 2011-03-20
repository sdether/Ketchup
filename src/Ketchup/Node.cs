using System;
using System.Configuration;
using System.Net.Sockets;
using Ketchup.Config;

namespace Ketchup {
	public class Node {

		private readonly byte[] _buffer = new byte[1024];
		private readonly object _sync = new object();
		private Socket _nodeSocket;

		public int Port { get; set; }
		public int CurrentRetryCount { get; set; }
		public bool IsDead { get; set; }
		public string Host { get; set; }
		public DateTime DeadAt { get; set; }
		public DateTime LastConnectionFailure { get; set; }

		public string Id {
			get { return Host + ":" + Port; }
		}

		private Socket NodeSocket {
			get {
				lock (_sync) {
					if (_nodeSocket == null)
						_nodeSocket = Connect(CreateSocket());
				}
				return Connect(_nodeSocket);
			}
		}

		public Node() {
			IsDead = false;
			CurrentRetryCount = -1;
			DeadAt = DateTime.MinValue;
			LastConnectionFailure = DateTime.MinValue;
		}

		public Node(string host, int port)
			: this() {
			Host = host;
			Port = port;
		}

		public void Request(byte[] packet, Action<byte[]> process) {
			try {
				var state = new NodeAsyncState() { Socket = NodeSocket, Process = process };
				state.Socket.BeginSend(packet, 0, packet.Length, SocketFlags.None, SendData, state);
			} catch {
				throw;
			}

		}

		private void SendData(IAsyncResult asyncResult) {
			try {
				var state = (NodeAsyncState)asyncResult.AsyncState;
				var remote = state.Socket;

				remote.EndSend(asyncResult);
				remote.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveData, state);
			} catch {
				throw;
			}

		}

		private void ReceiveData(IAsyncResult asyncResult) {
			try {
				var state = (NodeAsyncState)asyncResult.AsyncState;
				var remote = state.Socket;

				remote.EndReceive(asyncResult);
				if (state.Process != null) state.Process(_buffer);
			} catch {
				throw;
			}

		}

		private Socket Connect(Socket socket) {
			if (socket != null && socket.Connected) return socket;
			if (IsDead) throw new Exception("Node is dead");
			if (socket == null) socket = CreateSocket();

			if (string.IsNullOrEmpty(Host)) throw new Exception("Host is null or empty string, cannot connect to socket");
			if (Port == default(int)) throw new Exception("Port is not valid for this Node, cannot connect to socket");

			try {
				//if a connection error happened before, recurse until reconnect time has passed
				if (!ReadyToTry()) Connect(socket);

				//the big kahuna
				socket.Connect(Host, Port);

			} catch (SocketException ex) {
				//at least retry timeouts the specified number of times
				if (ex.SocketErrorCode == SocketError.TimedOut) return HandleTimeout(socket);
				throw;
			}

			return socket;
		}

		private static Socket CreateSocket() {
			var config = KetchupConfig.Current;

			return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) {
				SendTimeout = (int)config.ConnectionTimeout.TotalMilliseconds,
				ReceiveTimeout = (int)config.ConnectionTimeout.TotalMilliseconds,
				NoDelay = true,
				Blocking = true,
				UseOnlyOverlappedIO = false
			};
		}

		private bool ReadyToTry() {
			if (LastConnectionFailure == DateTime.MinValue) return true;
			return (DateTime.Now - LastConnectionFailure) >= KetchupConfig.Current.ConnectionRetryDelay;
		}

		private Socket HandleTimeout(Socket socket) {
			LastConnectionFailure = DateTime.Now;

			if (CurrentRetryCount++ >= KetchupConfig.Current.ConnectionRetryCount) {
				IsDead = true;
				DeadAt = LastConnectionFailure;
				return socket;
			}

			//haven't reached the max retries yet, let's try again until we do
			return Connect(socket);
		}

		public static int GetPort(string endpoint) {
			int port;
			var portString = endpoint.Contains(":") ? endpoint.Split(':')[1] : "11211";
			if (!int.TryParse(portString, out port)) throw new ConfigurationErrorsException("The specified port is not a valid int integer");
			return port;
		}

		private class NodeAsyncState {
			public Socket Socket { get; set; }
			public Action<byte[]> Process { get; set; }
		}
	}
}


