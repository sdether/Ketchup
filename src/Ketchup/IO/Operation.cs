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
		private byte[]						_result;
		
		public int							TotalSize { get; set; }
		public byte[]						Packet { get; set; }
		public byte[]						Buffer { get; set; }
		public object						State { get; private set; }

		public readonly IList<byte[]>		Buffers = new List<byte[]>();
		public Action<byte[], object>		Process { get; private set; }
		public Action<Exception, object>	Error { get; private set; }
		public Socket						Socket { get; set; }
		public EventLoop					EventLoop { private get; set; }

		public byte[] Result {
			get { return _result ?? (_result = Buffers.ToByteArray(TotalSize)); }
		}

		public Operation(byte[] packet, Socket socket)
		{
			Socket = socket;
			Packet = packet;
		}

		public Operation(byte[] packet, Socket socket, 
			Action<byte[], object> process, 
			Action<Exception, object> error, 
			object state) : this(packet, socket)
		{
			Process = process;
			Error = error;
			State = state;
		}

		public Operation QueueProcess() 
		{
			EventLoop.QueueProcess(this);
			return this;
		}
	}
}
