using System.Collections.Concurrent;
using System.Threading;

namespace Ketchup
{
	public class EventLoop
	{
		private readonly ManualResetEventSlim _handle = new ManualResetEventSlim(true);
		private readonly ConcurrentQueue<Operation> _sendQueue = new ConcurrentQueue<Operation>();
		private readonly ConcurrentQueue<Operation> _processQueue = new ConcurrentQueue<Operation>();

		public static void Run(object state)
		{
			var loop = (EventLoop)state;
			while (true)
			{
				loop.WaitIfEmpty();
				loop.Process();
				loop.Send();
			}
		}

		public void QueueSend(Operation op)
		{
			op.EventLoop = this;
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
			if (_processQueue.IsEmpty && _sendQueue.IsEmpty)
				_handle.Wait();
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
			if (!_sendQueue.TryDequeue(out op)) return;
			op.Send();
		}
	}
}
