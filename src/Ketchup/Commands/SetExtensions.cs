using System;
using Ketchup.Silent;

namespace Ketchup
{
	public static class SetExtensions
	{
		public static void Set<T>(this Bucket bucket, string key, T value) 
		{
			SilentSetExtensions.Set(bucket, key, value);
		}

		public static void Set<T>(this Bucket bucket, string key, T value, TimeSpan expiration) 
		{
			SilentSetExtensions.Set(bucket, key, value, expiration);
		}

		public static void Set<T>(this Bucket bucket, string key, T value, DateTime expiration) 
		{
			SilentSetExtensions.Set(bucket, key, value, expiration);
		}
	}
}
