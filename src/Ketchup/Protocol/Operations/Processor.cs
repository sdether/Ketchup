using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Ketchup.Protocol.Operations
{
	public class Processor
	{
		public Node Node { get; private set; }

		public Processor(Node node)
		{
			Node = node;
		}

		public void Start()
		{
			var thread = new Thread(state =>
			{
				while (true)
				{
					var processor = (Processor)state;

					//if there's nothing in the queue to send skip Send
					if (!processor.Node.WriteQueue.IsEmpty)
						processor.Send();

					//likewise, skip Receive if nothing in the queue
					if (!processor.Node.ReadQueue.IsEmpty)
						processor.Receive();

					//process any data that is in the Process queue
					if (!processor.Node.ProcessQueue.IsEmpty)
						processor.Process();
				}

			});
			thread.Start(this);
		}

		public void Send()
		{
			//pull enough operations out of the queue to fill the buffer
			var sbuffer = Buffer.Fill(Node.WriteQueue, Node.ReadQueue);
			var sstate = new SendState
			{
				Socket = Node.NodeSocket
			};

			sstate.Socket.BeginSend(
				sbuffer, 0, sbuffer.Length,
				SocketFlags.None, SendData, sstate
			);
		}

		public void Receive()
		{
			var rbuffer = Buffer.GetReceiveBuffer();
			var rstate = new ReceiveState
			{
				Processor = this,
				Socket = Node.NodeSocket,
				ReceiveBuffer = rbuffer
			};

			rstate.Socket.BeginReceive(
				rbuffer, 0, rbuffer.Length,
				SocketFlags.None, ReceiveData, rstate
			);
		}

		public void Process() 
		{
			Operation op;
			if (!Node.ProcessQueue.TryDequeue(out op)) return;
			op.Process(op.Response, op.State);
		}

		private static void SendData(IAsyncResult asyncResult)
		{
			var state = (SendState)asyncResult.AsyncState;
			var remote = state.Socket;
			remote.EndSend(asyncResult);
		}

		private static void ReceiveData(IAsyncResult asyncResult)
		{
			var state = (ReceiveState)asyncResult.AsyncState;
			var remote = state.Socket;
			remote.EndReceive(asyncResult);

			var readQueue = state.Processor.Node.ReadQueue;
			var processQueue = state.Processor.Node.ProcessQueue;
			Buffer.Drain(state.ReceiveBuffer, readQueue, processQueue);
		}

		private class SendState
		{
			public Socket Socket { get; set; }
		}

		private class ReceiveState
		{
			public Processor Processor { get; set; }
			public Socket Socket { get; set; }
			public byte[] ReceiveBuffer { get; set; }
		}

	}
}
