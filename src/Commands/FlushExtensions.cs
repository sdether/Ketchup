using Ketchup.Silent;

namespace Ketchup.Commands
{
	public static class FlushExtensions
	{
		public static void Flush(this Bucket bucket) 
		{
			SilentFlushExtensions.Flush(bucket);
		}
	}
}
