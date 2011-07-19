using Ketchup.Async;

namespace Ketchup.Sync
{
	public static class SyncFlushExtensions
	{
		public static bool Flush(this Bucket bucket)
		{
			var value = default(bool);

			SyncExtensions.ExecuteSync("", "FLUSH", (success, fail) =>
			{
				AsyncFlushExtensions.Flush(bucket, state =>
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
