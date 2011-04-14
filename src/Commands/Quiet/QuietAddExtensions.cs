using System;
using Ketchup.Async;

namespace Ketchup.Quiet {

	public static class QuietAddExtensions {

		public static Bucket Add<T>(this Bucket bucket, string key, 
			T value, 
			Action<Exception, object> error, object state)
		{
			return AsyncAddExtensions.Add(bucket, key, value, null, error, state);
		}

		public static Bucket Add<T>(this Bucket bucket, string key, 
			T value, TimeSpan expiration, 
			Action<Exception, object> error, object state)
		{
			return AsyncAddExtensions.Add(bucket, key, value, expiration, null, error, state);
		}

		public static Bucket Add<T>(this Bucket bucket, string key, 
			T value, DateTime expiration,
			Action<Exception, object> error, object state)
		{
			return AsyncAddExtensions.Add(bucket, key, value, expiration, null, error, state);
		}
	}
}

