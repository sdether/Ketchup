using Ketchup.Async;

namespace Ketchup.Silent
{
	public static class SilentDeleteExtensions
	{
		public static Bucket Delete(this Bucket bucket, string key)
		{
			return AsyncDeleteExtensions.Delete(bucket, key, null, null, null);
		}
	}
}

