using System;
using Ketchup.Async;

namespace Ketchup.Quiet
{
	public static class QuietDeleteExtensions
	{
		public static Bucket Delete(this Bucket bucket, string key,
			Action<Exception, object> error, object state)
		{
			return AsyncDeleteExtensions.Delete(bucket, key, null, error, state);
		}
	}
}

