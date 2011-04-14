using System;
using Ketchup.Async;

namespace Ketchup.Silent
{
	public static class SilentSetExtensions
	{
		public static Bucket Set<T>(this Bucket bucket, string key, T value)
		{
			return AsyncSetExtensions.Set(bucket, key, value, null, null, null);
		}

		public static Bucket Set<T>(this Bucket bucket, string key, T value, TimeSpan expiration)
		{
			return AsyncSetExtensions.Set(bucket, key, value, expiration, null, null, null);
		}

		public static Bucket Set<T>(this Bucket bucket, string key, T value, DateTime expiration)
		{
			return AsyncSetExtensions.Set(bucket, key, value, expiration, null, null, null);
		}
	}
}

