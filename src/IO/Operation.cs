using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Ketchup.IO
{
	/// <summary>
	/// An operation is a DTO that represents a message sent to the server with a byte array and a callback
	/// </summary>
	public class Operation
	{
		private byte[] _result;
		private Socket _socket;

		public int TotalSize { get; set; }
		public byte[] Packet { get; set; }
		public byte[] Buffer { get; set; }
		public object State { get; private set; }

		public readonly IList<byte[]> Buffers = new List<byte[]>();
		public Action<byte[], object> Process { get; private set; }
		public Action<Exception, object> Error { get; private set; }
		public EventLoop EventLoop { private get; set; }
		public Node Node { get; set; }

		public Socket Socket 
		{
			get { return _socket ?? (_socket = Node.GetSocketFromPool());}
		}

		public byte[] Result
		{
			get { return _result ?? (_result = Buffers.ToByteArray(TotalSize)); }
		}

		public Operation(Node node, byte[] packet)
		{
			Node = node;
			Packet = packet;
		}

		public Operation(Node node, byte[] packet, 
			Action<byte[], object> process,
			Action<Exception, object> error,
			object state)
			: this(node, packet)
		{
			Process = process;
			Error = error;
			State = state;
		}
		
		public Operation BeginSend()
		{
			SocketIO.BeginSend(this);
			return this;
		}
		
		public Operation OnSend()
		{
			//TODO: Use this method to timeout the socket if no response received
			SocketIO.BeginReceive(this);
			return this;
		}
		
		public Operation OnReceive()
		{
			if(EventLoop != null) EventLoop.QueueProcess(this);
			else Process(Result,State);
			Node.ReleaseSocket(_socket);
			_socket = null;
			return this;
		}
	}
}
