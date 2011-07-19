using System;
using Ketchup.Protocol.Commands;

namespace Ketchup.Async
{
	public static class AsyncFlushExtensions
	{
		public static Bucket Flush(this Bucket bucket,
			Action<object> success, Action<Exception, object> error, object state)
		{
			return new FlushCommand(bucket)
			{
				State = state,
				Error = error,
				Success = success
			}.Flush();
		}
	}
}
