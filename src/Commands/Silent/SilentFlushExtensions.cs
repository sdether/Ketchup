using Ketchup.Async;

namespace Ketchup.Silent
{
	public static class SilentFlushExtensions
	{
		public static Bucket Flush(this Bucket bucket)
		{
			return AsyncFlushExtensions.Flush(bucket, null, null, null);
		}
	}
}

