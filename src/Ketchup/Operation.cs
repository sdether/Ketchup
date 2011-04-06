using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Ketchup.Hashing;

namespace Ketchup
{
	/// <summary>
	/// An operation is a DTO that represents a message sent to the server with a byte array and a callback
	/// </summary>
	public class Operation
	{
		private const int _size = 1024;
		private readonly IList<byte[]> _buffers = new List<byte[]>();
		private readonly byte[] _packet;
		private readonly Node _node;
		private int _totalSize = 0;
		private byte[] _buffer;
		private byte[] _result;
		private Socket _socket;

		public EventLoop EventLoop { private get; set; }
		public Action<byte[], object> Process { get; private set; }
		public Action<Exception, object> Error { get; private set; }
		public object State { get; private set; }

		public Socket Socket {
			get { return _socket ?? (_socket = _node.NodeSocket); }
		}

		public byte[] Result {
			get { return _result ?? (_result = _buffers.ToByteArray(_totalSize)); }
		}

		public Operation(byte[] packet, Node node, 
			Action<byte[], object> process, 
			Action<Exception, object> error, 
			object state) 
		{
			_node = node;
			_packet = packet;
			Process = process;
			Error = error;
			State = state;
		}

		public Operation(byte[] packet, string key, string bucket, 
			Action<byte[], object> process, 
 			Action<Exception, object> error, 
			object state) 
		{
			_node = Hasher.GetNode(key, bucket);
			_packet = packet;
			Process = process;
			Error = error;
			State = state;
		}

		public void Send() 
		{
			Socket.BeginSend(
				_packet, 0, _packet.Length,
				SocketFlags.None, SendData, this
			);
			Receive();
		}

		private void Receive() 
		{
			_buffer = new byte[_size];
			Socket.BeginReceive(
				_buffer, 0, _size,
				SocketFlags.None, ReceiveData, this
			);
		}

		private static void SendData(IAsyncResult asyncResult)
		{
			var op = (Operation)asyncResult.AsyncState;
			op.Socket.EndSend(asyncResult);
		}

		private static void ReceiveData(IAsyncResult asyncResult)
		{
			var op = (Operation)asyncResult.AsyncState;
			var read = op.Socket.EndReceive(asyncResult);
			op._buffers.Add(op._buffer);
			op._totalSize += read;
			if (read < _size) 
				op.EventLoop.QueueProcess(op);
			else 
				op.Receive();
		}
	}
}
