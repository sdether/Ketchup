using System;
using Ketchup.Async;

namespace Ketchup.Silent
{
	public static class SilentReplaceExtensions
	{
		public static Bucket Replace<T>(this Bucket bucket, string key, T value)
		{
			return AsyncReplaceExtensions.Replace(bucket, key, value, null, null, null);
		}

		public static Bucket Replace<T>(this Bucket bucket, string key, T value, TimeSpan expiration)
		{
			return AsyncReplaceExtensions.Replace(bucket, key, value, expiration, null, null, null);
		}

		public static Bucket Replace<T>(this Bucket bucket, string key, T value, DateTime expiration)
		{
			return AsyncReplaceExtensions.Replace(bucket, key, value, expiration, null, null, null);
		}
	}
}

