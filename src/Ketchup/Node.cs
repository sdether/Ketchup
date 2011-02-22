﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Ketchup.Config;
using System.Threading;

namespace Ketchup {
	public class Node {
		private Socket socket = null;

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

		public bool Connect() {
			KetchupConfig config = KetchupConfig.Current;
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			//TODO determine if nodelay from enyim actually makes a difference
			//socket.NoDelay = true;

			if (string.IsNullOrEmpty(Host))
				throw new ArgumentNullException("Host is null or empty string, cannot connect to socket");

			if (Port == default(int))
				throw new ArgumentNullException("Port is not valid for this Node, cannot connect to socket");

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
						return HandleTimeout();
					default:
						throw ex;
				}
			}

			return true;
		}

		private bool ReadyToTry() {
			KetchupConfig config = KetchupConfig.Current;
			if(LastConnectionFailure == DateTime.MinValue)
				return true;
			
			if((DateTime.Now - LastConnectionFailure) < config.ConnectionRetryDelay )
				return false;

			return true;
		}

		private bool HandleTimeout() {
			KetchupConfig config = KetchupConfig.Current;
			LastConnectionFailure = DateTime.Now;
			
			//haven't reached the max retries yet, let's try again until we do
			if (CurrentRetryCount < config.ConnectionRetryCount){
				CurrentRetryCount++;
				return Connect();
			}

			IsDead = true;
			DeadAt = LastConnectionFailure;
			socket = null;
			return false;
		}

		public Node Request(byte[] packet, Action<byte[]> callback, Action<Exception> error) {
			if(IsDead) 
				throw new Exception("Node is dead");

			//TODO: Make connect async
			if (socket == null)
				if (!Connect()) throw new Exception("Connect Failed");

			
			socket.BeginSend(packet, 0, packet.Length, SocketFlags.None,
				sendState => {
					try {
						((Socket)sendState.AsyncState).EndSend(sendState);
					} catch(Exception ex) {
						error(ex);
					}
				},socket);

			var buffer = new byte[1024];
			socket.BeginReceive(buffer, 0, 1024, SocketFlags.None,
				receiveState => {
					try {
						((Socket)receiveState.AsyncState).EndReceive(receiveState);
						callback(buffer);
					} catch (Exception ex) {
						error(ex);
					}
				}, socket);

			return this;
		}
	}
}

