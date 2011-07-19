using System;
using System.Net.Sockets;
using System.Threading;

namespace Ketchup.IO
{
	public static class SocketIO
	{
		private static readonly int _size = Config.KetchupConfig.Current.BufferSize;

		public static void Send(Operation op)
		{
			op.Socket.Send(op.Packet, 0, op.Packet.Length, SocketFlags.None);
			Receive(op);
		}
		
		private static void Receive(Operation op)
		{
			op.Buffer = new byte[_size];
			var read = op.Socket.Receive(op.Buffer, 0, op.Buffer.Length, SocketFlags.None);
			op.Buffers.Add(op.Buffer);
			op.TotalSize += read;
			if (read < _size) op.OnReceive();
			else Receive(op);
		}

		public static void BeginSend(Operation op)
		{
			SocketError error;
			op.Socket.BeginSend(
				op.Packet, 0, op.Packet.Length,
				SocketFlags.None, out error, OnDataSent, op);
		}
		
		private static void OnDataSent(IAsyncResult asyncResult)
		{
			FakeNetwork();
			var op = (Operation)asyncResult.AsyncState;
			op.Socket.EndSend(asyncResult);
			op.OnSend();
		}

		public static void BeginReceive(Operation op)
		{
			op.Buffer = new byte[_size];
			op.Socket.BeginReceive(
				op.Buffer, 0, op.Buffer.Length,
				SocketFlags.None, OnDataReceived, op);
		}


		private static void OnDataReceived(IAsyncResult asyncResult)
		{
			FakeNetwork();
			var op = (Operation)asyncResult.AsyncState;
			var read = op.Socket.EndReceive(asyncResult);
			op.Buffers.Add(op.Buffer);
			op.TotalSize += read;
			if (read < _size) op.OnReceive();
			else Receive(op);
		}
		
		private static void FakeNetwork()
		{
			//pick a random number and sleep for that amount
			var random = new Random();
			var max = 10;
			var min = 50;
			
    		var latency = random.NextDouble() * (max - min) + min;
			var millisec = Convert.ToInt32(latency);
			Thread.Sleep(millisec);
		}
	}
}
