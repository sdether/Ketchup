using System;
using Ketchup.Async;

namespace Ketchup.Quiet {

	public static class QuietReplaceExtensions {

		public static Bucket Replace<T>(this Bucket bucket, string key, 
			T value, 
			Action<Exception, object> error, object state)
		{
			return AsyncReplaceExtensions.Replace(bucket, key, value, null, error, state);
		}

		public static Bucket Replace<T>(this Bucket bucket, string key, 
			T value, TimeSpan expiration, 
			Action<Exception, object> error, object state)
		{
			return AsyncReplaceExtensions.Replace(bucket, key, value, expiration, null, error, state);
		}

		public static Bucket Replace<T>(this Bucket bucket, string key, 
			T value, DateTime expiration,
			Action<Exception, object> error, object state)
		{
			return AsyncReplaceExtensions.Replace(bucket, key, value, expiration, null, error, state);
		}
	}
}

