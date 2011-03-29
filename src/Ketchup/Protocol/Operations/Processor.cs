using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Ketchup.Protocol.Operations
{
	public class Processor
	{
		private readonly object _sync = new object();

		public bool Started { get; set; }
		public Node Node { get; private set; }
		public ConcurrentQueue<Operation> WriteQueue = new ConcurrentQueue<Operation>();
		public ConcurrentQueue<Operation> ReadQueue = new ConcurrentQueue<Operation>();
		public ConcurrentQueue<Operation> ProcessQueue = new ConcurrentQueue<Operation>();

		public Processor(Node node)
		{
			Node = node;
		}

		public void Start()
		{
			lock (_sync)
			{
				if (Started) return;

				var thread = new Thread(state =>
				{
					while (true)
					{
						var processor = (Processor)state;

						//if there's nothing in the queue to send skip Send
						if (!processor.WriteQueue.IsEmpty)
							processor.Send();

						//likewise, skip Receive if nothing in the queue
						if (!processor.ReadQueue.IsEmpty)
							processor.Receive();

						//process any data that is in the Process queue
						if (!processor.ProcessQueue.IsEmpty)
							processor.Process();
					}

				});
				thread.Start(this);
				Started = true;
			}
		}

		public void Send()
		{
			//pull enough operations out of the queue to fill the buffer
			var sbuffer = Buffer.Fill(WriteQueue, ReadQueue);
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
			if (!ProcessQueue.TryDequeue(out op)) return;
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

			var readQueue = state.Processor.ReadQueue;
			var processQueue = state.Processor.ProcessQueue;
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
