using System.Collections.Concurrent;
using System.Threading;

namespace Ketchup.IO
{
	public class EventLoop
	{
		private readonly ManualResetEventSlim _handle = new ManualResetEventSlim(true);
		private readonly ConcurrentQueue<Operation> _sendQueue = new ConcurrentQueue<Operation>();
		private readonly ConcurrentQueue<Operation> _processQueue = new ConcurrentQueue<Operation>();
		
		public EventLoop()
		{
			Start();
		}

		private EventLoop Start()
		{
			new Thread(Run)
			{
				IsBackground = true,
				Name = "Ketchup.IO.EventLoop"
			}.Start(this);
			
			return this;
		}

		private static void Run(object state)
		{
			var loop = (EventLoop)state;
			while (true)
			{
				//loop.WaitIfEmpty();
				Thread.Sleep(10);
				loop.Process();
				loop.Send();
			}
		}

		public void QueueSend(Operation op)
		{
			_sendQueue.Enqueue(op);
			_handle.Set();
		}

		public void QueueProcess(Operation op) 
		{
			_processQueue.Enqueue(op);
			_handle.Set();
		}

		private void WaitIfEmpty() 
		{
			_handle.Reset();
			if (_processQueue.IsEmpty && _sendQueue.IsEmpty) _handle.Wait();
		}

		private void Process()
		{
			Operation op;
			if (!_processQueue.TryDequeue(out op)) return;
			op.Process(op.Result, op.State);
		}

		private void Send()
		{
			Operation op;
			_sendQueue.TryDequeue(out op);
			if(op != null) 
				SocketIO.Send(op);
		}
	}
}
