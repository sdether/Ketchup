using System;
using System.Threading;
using Xunit;
using Ketchup.Sync;
using Ketchup.Protocol.Exceptions;

namespace Ketchup.Tests.Commands
{
	public class SetTests
	{
		private readonly static Bucket bucket = TestHelpers.Bucket;

		[Fact]
		public void SetWithSuccess()
		{
			var key = "set-success";
			var value = key + "-value";

			var success = bucket.Set(key, value);
			Assert.True(success);

			var actual = bucket.Get<string>(key);
			Assert.Equal(value, actual);

			success = bucket.Delete(key);
			Assert.True(success);
		}

		[Fact]
		public void SetWithException()
		{
			var key = "set-exception";
			var value = new byte[2000000];
			Assert.Throws<ValueTooLargeException>(() => bucket.Set(key, value));
		}

		[Fact]
		public void SetWithExpirationTest()
		{
			var key = "set-expiration-success";
			var value = key + "-value";
			
			var success = bucket.Set(key, value, new TimeSpan(0, 0, 1));
			Assert.True(success);
			Thread.Sleep(2 * 1000);
			
			var result = bucket.Get<string>(key);
			Assert.Null(result);
		}

		[Fact]
		public void ReplaceWithException()
		{
			var key = "replace-exception";
			var value = key + "-value";
			Assert.Throws<NotFoundException>(() => bucket.Replace(key, value));
		}

		[Fact]
		public void AddWithException()
		{
			var key = "add-exception";
			var value = key + "-value";

			var success = bucket.Set(key, value);
			Assert.True(success);

			//then try to add a value with the same key
			Assert.Throws<KeyExistsException>(() => bucket.Add(key, value));

			success = bucket.Delete(key);
			Assert.True(success);
		}


	}
}
