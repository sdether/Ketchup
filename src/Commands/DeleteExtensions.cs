using Ketchup.Silent;

namespace Ketchup.Commands
{
	public static class DeleteExtensions
	{
		public static void Delete(this Bucket bucket, string key) 
		{
			SilentDeleteExtensions.Delete(bucket, key);
		}
	}
}
