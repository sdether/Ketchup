using System;
using Ketchup.Async;

namespace Ketchup.Quiet
{
	public static class QuietGetExtensions
	{
		public static Bucket Get<T>(this Bucket bucket, string key,
			Action<T, object> hit, Action<Exception, object> error, object state)
		{
			return AsyncGetExtensions.Get(bucket, key, hit, null, error, state);
		}
	}
}

