using Ketchup.Async;

namespace Ketchup.Sync
{
	public static class SyncDeleteExtensions
	{
		public static bool Delete(this Bucket bucket, string key)
		{
			var value = default(bool);

			SyncExtensions.ExecuteSync(key, "DELETE", (success, fail) =>
			{
				AsyncDeleteExtensions.Delete(bucket, key, state =>
				{
					value = true;
					success(state);
				}, 
				fail, null);
			});

			return value;
		}
	}
}
