using System;
using System.Net.Sockets;

namespace Ketchup.IO
{
	public static class SocketIO
	{
		private static readonly int _size = Config.KetchupConfig.Current.BufferSize;

		public static void Send(Operation op)
		{
			op.Socket.BeginSend(
				op.Packet, 0, op.Packet.Length,
				SocketFlags.None, SendData, op
			);
			Receive(op);
		}

		private static void Receive(Operation op)
		{
			op.Buffer = new byte[_size];
			op.Socket.BeginReceive(
				op.Buffer, 0, _size,
				SocketFlags.None, ReceiveData, op
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
			op.Buffers.Add(op.Buffer);
			op.TotalSize += read;
			if (read < _size) op.QueueProcess();
			else Receive(op);
		}
	}
}
