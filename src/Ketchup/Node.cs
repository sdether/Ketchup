using System;
using System.Configuration;
using System.Net.Sockets;
using Ketchup.Config;

namespace Ketchup {
	public class Node {
		//private Socket socket;

		public int		Port					{ get; set; }
		public int		CurrentRetryCount		{ get; set; }
		public bool		IsDead					{ get; set; }
		public string	Host					{ get; set; }
		public DateTime DeadAt					{ get; set; }
		public DateTime LastConnectionFailure	{ get; set; }
		

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

		public Socket Connect() {
			var config = KetchupConfig.Current;
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			//TODO determine if nodelay from enyim actually makes a difference
			//socket.NoDelay = true;

			if (string.IsNullOrEmpty(Host))
				throw new Exception("Host is null or empty string, cannot connect to socket");

			if (Port == default(int))
				throw new Exception("Port is not valid for this Node, cannot connect to socket");

			//if either the send or the receive times out, throw the connection timeout;
			socket.SendTimeout = socket.ReceiveTimeout = config.ConnectionTimeout.Milliseconds;

			try {
				//recurse until time has passed
				if(!ReadyToTry())
					return Connect();

				socket.Connect(Host, Port);

			} catch (SocketException ex) {
				switch (ex.SocketErrorCode) {
					case SocketError.TimedOut:
						return HandleTimeout(socket);
					default:
						throw;
				}
			}

			return socket;
		}

		private bool ReadyToTry() {
			var config = KetchupConfig.Current;
			if(LastConnectionFailure == DateTime.MinValue)
				return true;
			
			return (DateTime.Now - LastConnectionFailure) >= config.ConnectionRetryDelay;
		}

		private Socket HandleTimeout(Socket socket) {
			var config = KetchupConfig.Current;
			LastConnectionFailure = DateTime.Now;
			
			//haven't reached the max retries yet, let's try again until we do
			if (CurrentRetryCount < config.ConnectionRetryCount){
				CurrentRetryCount++;
				return Connect();
			}

			IsDead = true;
			DeadAt = LastConnectionFailure;
			
			return socket;
		}

		public Node Request(byte[] packet, Action<byte[]> callback) {
			if(IsDead) 
				throw new Exception("Node is dead");

			//TODO: Make connect async
			var socket = Connect();
			if(socket==null)
				throw new Exception("Connect Failed");

			socket.BeginSend(packet, 0, packet.Length, SocketFlags.None,
				sendState => {
					((Socket)sendState.AsyncState).EndSend(sendState);
				},socket);

			var buffer = new byte[1024];
			if (callback != null)
				socket.BeginReceive(buffer, 0, 1024, SocketFlags.None,
					receiveState => {
						var s = (Socket)receiveState.AsyncState;
						s.EndReceive(receiveState);
						callback(buffer);
					}, socket);

			return this;
		}

		public static int GetPort(string endpoint) {
			int port;

			var portString = endpoint.Contains(":") ? endpoint.Split(':')[1] : "11211";
			if (!int.TryParse(portString, out port))
				throw new ConfigurationErrorsException("The specified port is not a valid int integer");

			return port;
		}
	}
}


