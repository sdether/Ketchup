using System;
using System.Net.Sockets;

namespace Ketchup.Protocol.Operations
{
	/// <summary>
	/// An operation is a DTO that represents a message sent to the server with a byte array and a callback
	/// </summary>
	public class Operation
	{
		public byte[] Packet { get; set; }
		public byte[] Response { get; set; }
		public Action<byte[], object> Process { get; set; }
		public Action<Exception, object> Error { get; set; }
		public object State { get; set; }
		public Socket Socket { get; set; }
	}
}
