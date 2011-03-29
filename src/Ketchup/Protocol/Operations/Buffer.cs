using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ketchup.Protocol.Operations
{
	public static class Buffer
	{
		private static readonly int _bufferLength = 1024;
		private static readonly object _sync = new object();

		public static byte[] GetReceiveBuffer()
		{
			return new byte[_bufferLength];
		}

		public static List<byte> Drain(List<byte> buffer, ConcurrentQueue<Operation> readQueue, ConcurrentQueue<Operation> processQueue)
		{
			lock (_sync)
			{
				//now grab the first item from the readQueue
				Operation op;
				if (!readQueue.TryDequeue(out op))
					//no idea what to do with this error, need a generic error handler for the whole instance?
					throw new Exception("Bytes were received from the buffer but no operations were available in the ReadQueue");

				var responseLength = 0;
				try
				{
					//in memcached ordering is preserverd so the first response in the buffer 
					//should be the first item in the readQueue. I have 0 confidence this will work
					if (buffer[0] != (byte)Magic.Response) 
						throw new Exception("Magic byte is not the first byte in the response");

					//get the working set and send that back to the processor
					responseLength = new PacketHeader(buffer).PacketLength;
					op.Response = buffer.Take(responseLength).ToArray();

					//add the response back to the ProcessQueue to be handled by the Processor thread
					processQueue.Enqueue(op);
				}
				catch (Exception ex)
				{
					op.Error(ex, op.State);
				}

				//create the new buffer and call drain again
				return new List<byte>(buffer.Skip(responseLength));
			}
		}

		public static byte[] Fill(ConcurrentQueue<Operation> writeQueue, ConcurrentQueue<Operation> readQueue)
		{
			var fill = Fill(new List<byte>(), writeQueue, readQueue);
			var buffer = new byte[fill.Count];
			fill.CopyTo(buffer, 0);
			return buffer;
		}

		private static List<byte> Fill(List<byte> buffer, ConcurrentQueue<Operation> writeQueue, ConcurrentQueue<Operation> readQueue)
		{
			Operation op;

			//at this point there should never be nothing in the queue, but just in case
			if (!writeQueue.TryPeek(out op))
				return buffer;

			//check if adding the next item in the queue would overflow the buffer
			if (op.Packet.Length + buffer.Count > _bufferLength)
				return buffer;

			//again, if you peeked at it, it should still be here, so you should never hit this
			if (!writeQueue.TryDequeue(out op))
				return buffer;

			//make sure these two operations happen as transaction
			var currentIndex = buffer.Count;
			try
			{
				buffer.AddRange(op.Packet);
				readQueue.Enqueue(op);
			}
			catch
			{
				//roll it back
				buffer.RemoveRange(currentIndex, op.Packet.Length);
				writeQueue.Enqueue(op);
			}

			//continue filling the buffer until it's full
			return Fill(buffer, writeQueue, readQueue);
		}
	}
}
