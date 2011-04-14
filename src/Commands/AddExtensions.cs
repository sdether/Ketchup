using System;
using Ketchup.Silent;

namespace Ketchup.Commands
{
	public static class AddExtensions
	{
		public static void Add<T>(this Bucket bucket, string key, T value)
		{
			SilentAddExtensions.Add(bucket, key, value);
		}

		public static void Add<T>(this Bucket bucket, string key, T value, TimeSpan expiration)
		{
			SilentAddExtensions.Add(bucket, key, value, expiration);
		}

		public static void Add<T>(this Bucket bucket, string key, T value, DateTime expiration)
		{
			SilentAddExtensions.Add(bucket, key, value, expiration);
		}
	}
}
