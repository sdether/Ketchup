using System;
using Ketchup.Async;

namespace Ketchup.Sync
{
	public static class SyncReplaceExtensions
	{
		public static bool Replace<T>(this Bucket bucket, string key, T value)
		{
			var replacesuccess = default(bool);

			SyncExtensions.ExecuteSync(key, "REPLACE", (success, fail) =>
			{
				AsyncReplaceExtensions.Replace(bucket, key, value, state =>
				{
					replacesuccess = true;
					success(state);
				}, 
				fail, null);
			});

			return replacesuccess;
		}

		public static bool Replace<T>(this Bucket bucket, string key, T value, TimeSpan expiration)
		{
			var replacesuccess = default(bool);

			SyncExtensions.ExecuteSync(key, "REPLACE", (success, fail) =>
			{
				AsyncReplaceExtensions.Replace(bucket, key, value, expiration, state =>
				{
					replacesuccess = true;
					success(state);
				},
				fail, null);
			});

			return replacesuccess;
		}

		public static bool Replace<T>(this Bucket bucket, string key, T value, DateTime expiration)
		{
			var replacesuccess = default(bool);

			SyncExtensions.ExecuteSync(key, "REPLACE", (success, fail) =>
			{
				AsyncReplaceExtensions.Replace(bucket, key, value, expiration, state =>
				{
					replacesuccess = true;
					success(state);
				},
				fail, null);
			});

			return replacesuccess;
		}

	}
}
