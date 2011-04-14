using System;
using Ketchup.Async;

namespace Ketchup.Sync
{
	public static class SyncAddExtensions
	{
		public static bool Add<T>(this Bucket bucket, string key, T value)
		{
			var addsuccess = default(bool);

			SyncExtensions.ExecuteSync(key, "ADD", (success, fail) =>
			{
				AsyncAddExtensions.Add(bucket, key, value, state =>
				{
					addsuccess = true;
					success(state);
				}, 
				fail, null);
			});

			return addsuccess;
		}

		public static bool Add<T>(this Bucket bucket, string key, T value, TimeSpan expiration)
		{
			var addsuccess = default(bool);

			SyncExtensions.ExecuteSync(key, "SET", (success, fail) =>
			{
				AsyncAddExtensions.Add(bucket, key, value, expiration, state =>
				{
					addsuccess = true;
					success(state);
				},
				fail, null);
			});

			return addsuccess;
		}

		public static bool Add<T>(this Bucket bucket, string key, T value, DateTime expiration)
		{
			var addsuccess = default(bool);

			SyncExtensions.ExecuteSync(key, "SET", (success, fail) =>
			{
				AsyncAddExtensions.Add(bucket, key, value, expiration, state =>
				{
					addsuccess = true;
					success(state);
				},
				fail, null);
			});

			return addsuccess;
		}

	}
}
