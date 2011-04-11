using Ketchup.Async;

namespace Ketchup.Sync
{
	public static class SyncGetExtensions
	{
		public static T Get<T>(this Bucket bucket, string key)
		{
			var value = default(T);

			SyncExtensions.ExecuteSync(key, "GET", (success, fail) =>
			{
				AsyncGetExtensions.Get<T>(bucket, key, (val, sth) =>
				{
					value = val;
					success(sth);
				}, 
				success, fail, null);
			});

			return value;
		}

		public static object Get(this Bucket bucket, string key)
		{
			return Get<object>(bucket, key);
		}
	}
}
