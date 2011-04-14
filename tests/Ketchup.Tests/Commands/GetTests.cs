using Xunit;
using Ketchup.Sync;

namespace Ketchup.Tests.Commands
{
	public class GetTests
	{
		private static readonly Bucket bucket = TestHelpers.Bucket;

		[Fact]
		public void GetWithHit()
		{
			var key = "get-hit";
			var value = key + "-value";
			bucket.Set(key, value);
			var result = bucket.Get<string>(key);
			Assert.NotNull(result);
			Assert.Equal(value, result);
		}

		[Fact]
		public void GetWithMiss()
		{
			var key = "get-miss";
			bucket.Delete(key);
			var result = bucket.Get<string>(key);
			Assert.Null(result);
		}
	}
}
