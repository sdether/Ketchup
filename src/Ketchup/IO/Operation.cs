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

		public Operation(byte[] packet, Node node)
		{
			Node = node;
			Packet = packet;
		}

		public Operation(byte[] packet, Node node,
			Action<byte[], object> process,
			Action<Exception, object> error,
			object state)
			: this(packet, node)
		{
			Process = process;
			Error = error;
			State = state;
		}

		public Operation QueueProcess()
		{
			EventLoop.QueueProcess(this);
			Node.ReleaseSocket(_socket);
			return this;
		}
	}
}
