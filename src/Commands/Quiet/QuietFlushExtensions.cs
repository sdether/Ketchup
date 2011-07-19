using System;
using Ketchup.Async;

namespace Ketchup.Quiet
{
	public static class QuietFlushExtensions
	{
		public static Bucket Flush(this Bucket bucket,
			Action<Exception, object> error, object state)
		{
			return AsyncFlushExtensions.Flush(bucket, null, error, state);
		}
	}
}

