using Xunit;
using Ketchup.Sync;
using Ketchup.Protocol.Exceptions;

namespace Ketchup.Tests.Commands
{
	public class AsyncTests
	{
		private readonly static Bucket bucket = TestHelpers.Bucket;

		[Fact]
		public void DeleteWithSuccess()
		{
			var key = "delete-success";
			var value = key + "-value";
			
			var success = bucket.Set(key, value);
			Assert.True(success);
			
			success = bucket.Delete(key);
			Assert.True(success);

			var actual = bucket.Get(key);
			Assert.Null(actual);
		}

		[Fact]
		public void DeleteWithException()
		{
			var key = "delete-exception";
			Assert.Throws<NotFoundException>(() => bucket.Delete(key));
		}
	}
}
