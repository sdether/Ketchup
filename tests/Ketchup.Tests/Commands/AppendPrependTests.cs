using Ketchup.Sync;
using Ketchup.Protocol.Exceptions;
using Xunit;

namespace Ketchup.Tests.Commands
{
	public class AppendPrependTests
	{
		private readonly static Bucket bucket = TestHelpers.Bucket;

		[Fact]
		public void AppendWithSuccess()
		{
			var key = "append-success";
			var value = key + "-value";
			var append = "-append";
			var expected = value + append;

			var success = bucket.Set(key, value);
			Assert.True(success);

			success = bucket.Append(key, append);
			Assert.True(success);

			var actual = bucket.Get<string>(key);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void AppendWithException()
		{
			var key = "append-exception";
			var append = "-append";
			Assert.Throws<ItemNotStoredException>(() => bucket.Append(key, append));
		}

		[Fact]
		public void PrependWithSuccess()
		{
			var key = "prepend-success";
			var value = key + "-value";
			var prepend = "prepend-";
			var expected = prepend + value;

			var success = bucket.Set(key, value);
			Assert.True(success);

			success = bucket.Prepend(key, prepend);
			Assert.True(success);

			var actual = bucket.Get<string>(key);
			Assert.Equal(expected, actual);
		}
	}
}
