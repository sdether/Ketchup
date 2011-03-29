using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Ketchup.Protocol.Operations
{
	public class Processor
	{
		private static readonly object _sync = new object();

		public bool Started { get; set; }
		public Node Node { get; private set; }
		public ConcurrentQueue<Operation> WriteQueue = new ConcurrentQueue<Operation>();
		public ConcurrentQueue<Operation> ReadQueue = new ConcurrentQueue<Operation>();
		public ConcurrentQueue<Operation> ProcessQueue = new ConcurrentQueue<Operation>();
		public List<byte> ReadBuffer = new List<byte>();

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
					var processor = (Processor)state;

					while (true)
					{
						//if there's nothing in the queue to send skip Send
						if (!processor.WriteQueue.IsEmpty)
							processor.Send();

						//likewise, skip Receive if nothing in the queue
						if (!processor.ReadQueue.IsEmpty)
							processor.Receive();

						//drain any data received from the socket
						if (processor.ReadBuffer.Count > 0)
							processor.Drain();

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

			//process thread loop handles async, not socket
			//Node.NodeSocket.Send(sbuffer);

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
			//var read = Node.NodeSocket.Receive(rbuffer);
			//ReadBuffer.AddRange(rbuffer.Take(read));

			var rstate = new ReceiveState
			{
				Processor = this,
				Socket = Node.NodeSocket,
				ReceiveBuffer = Buffer.GetReceiveBuffer(),
			};

			rstate.Socket.BeginReceive(
				rstate.ReceiveBuffer, 0, rstate.ReceiveBuffer.Length,
				SocketFlags.None, ReceiveData, rstate
			);
		}

		public void Drain()
		{
			lock (_sync) ReadBuffer = Buffer.Drain(ReadBuffer, ReadQueue, ProcessQueue);
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
			lock (_sync)
			{
				var rstate = (ReceiveState)asyncResult.AsyncState;
				var remote = rstate.Socket;
				var read = remote.EndReceive(asyncResult);
				rstate.Processor.ReadBuffer.AddRange(rstate.ReceiveBuffer.Take(read));
			}
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
