using System;
using Ketchup.Async;

namespace Ketchup.Quiet {

	public static class QuietSetExtensions {

		public static Bucket Set<T>(this Bucket bucket, string key, 
			T value, 
			Action<Exception, object> error, object state)
		{
			return AsyncSetExtensions.Set(bucket, key, value, null, error, state);
		}

		public static Bucket Set<T>(this Bucket bucket, string key, 
			T value, TimeSpan expiration, 
			Action<Exception, object> error, object state)
		{
			return AsyncSetExtensions.Set(bucket, key, value, expiration, null, error, state);
		}

		public static Bucket Set<T>(this Bucket bucket, string key, 
			T value, DateTime expiration,
			Action<Exception, object> error, object state)
		{
			return AsyncSetExtensions.Set(bucket, key, value, expiration, null, error, state);
		}
	}
}

