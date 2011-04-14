using System;
using Ketchup.Async;

namespace Ketchup.Sync
{
	public static class SyncSetExtensions
	{
		public static bool Set<T>(this Bucket bucket, string key, T value)
		{
			var setsuccess = default(bool);

			SyncExtensions.ExecuteSync(key, "SET", (success, fail) =>
			{
				AsyncSetExtensions.Set(bucket, key, value, state =>
				{
					setsuccess = true;
					success(state);
				}, 
				fail, null);
			});

			return setsuccess;
		}

		public static bool Set<T>(this Bucket bucket, string key, T value, TimeSpan expiration)
		{
			var setsuccess = default(bool);

			SyncExtensions.ExecuteSync(key, "SET", (success, fail) =>
			{
				AsyncSetExtensions.Set(bucket, key, value, expiration, state =>
				{
					setsuccess = true;
					success(state);
				},
				fail, null);
			});

			return setsuccess;
		}

		public static bool Set<T>(this Bucket bucket, string key, T value, DateTime expiration)
		{
			var setsuccess = default(bool);

			SyncExtensions.ExecuteSync(key, "SET", (success, fail) =>
			{
				AsyncSetExtensions.Set(bucket, key, value, expiration, state =>
				{
					setsuccess = true;
					success(state);
				},
				fail, null);
			});

			return setsuccess;
		}

	}
}
