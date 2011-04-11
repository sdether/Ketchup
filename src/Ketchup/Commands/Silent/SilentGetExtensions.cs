using System;
using Ketchup.Async;

namespace Ketchup.Silent
{
	public static class SilentGetExtensions
	{
		public static Bucket Get<T>(this Bucket bucket, string key, Action<T, object> hit, object state)
		{
			return AsyncGetExtensions.Get(bucket, key, hit, null, null, state);
		}
	}
}

