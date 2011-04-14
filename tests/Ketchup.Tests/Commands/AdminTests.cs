using Xunit;
using Ketchup.Sync;

namespace Ketchup.Tests.Commands 
{
	public class AdminTests
	{
		private static readonly Bucket bucket = TestHelpers.Bucket;

		[Fact]
		public void FlushTest() 
		{
			var key = "flush-success";
			var value = key + "-value";

			var success = bucket.Set(key, value);
			Assert.True(success);

			var result = bucket.Get<string>(key);
			Assert.Equal(value, result);

			success = bucket.Flush();
			Assert.True(success);

			result = bucket.Get<string>(key);
			Assert.Null(result);
		}


		[Fact]
		public void VersionTest()
		{
			var version = bucket.Version();
			Assert.NotNull(version);
			Assert.True(version.Length > 0);
		}

		[Fact]
		public void NoOpTest() {
			var success = bucket.NoOp();
			Assert.True(success);
		}

		[Fact]
		public void QuitTest() {
			var success = bucket.Quit();
			Assert.True(success);
		}
	}
}
