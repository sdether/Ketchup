using System;
using Ketchup.Silent;

namespace Ketchup.Commands
{
	public static class ReplaceExtensions
	{
		public static void Replace<T>(this Bucket bucket, string key, T value)
		{
			SilentReplaceExtensions.Replace(bucket, key, value);
		}

		public static void Replace<T>(this Bucket bucket, string key, T value, TimeSpan expiration)
		{
			SilentReplaceExtensions.Replace(bucket, key, value, expiration);
		}

		public static void Replace<T>(this Bucket bucket, string key, T value, DateTime expiration)
		{
			SilentReplaceExtensions.Replace(bucket, key, value, expiration);
		}
	}
}
