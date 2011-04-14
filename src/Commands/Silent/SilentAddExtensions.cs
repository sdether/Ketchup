using System;
using Ketchup.Async;

namespace Ketchup.Silent
{
	public static class SilentAddExtensions
	{
		public static Bucket Add<T>(this Bucket bucket, string key, T value)
		{
			return AsyncAddExtensions.Add(bucket, key, value, null, null, null);
		}

		public static Bucket Add<T>(this Bucket bucket, string key, T value, TimeSpan expiration)
		{
			return AsyncAddExtensions.Add(bucket, key, value, expiration, null, null, null);
		}

		public static Bucket Add<T>(this Bucket bucket, string key, T value, DateTime expiration)
		{
			return AsyncAddExtensions.Add(bucket, key, value, expiration, null, null, null);
		}
	}
}

