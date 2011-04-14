using System;
using Xunit;
using Ketchup.Sync;
using Ketchup.Protocol.Exceptions;

namespace Ketchup.Tests.Commands
{
	public class IncrDecrTests
	{
		private static readonly Bucket bucket = TestHelpers.Bucket;

		[Fact]
		public void IncrWithSuccess()
		{
			//first set intial value, step and expected result
			var key = "incr-success";
			long initial = 20;
			long step = 8;
			var result = initial + step;

			//call incrdecr first to set initial value, there's no step yet
			var actual = bucket.IncrDecr(key, initial, new TimeSpan(1, 0, 0));
			Assert.Equal(initial, actual);

			//now call incr/decr again with an 8 step to ensure you get the final result back.
			actual = bucket.IncrDecr(key, step);
			Assert.Equal(result, actual);

			//clear key
			bucket.Delete(key);
		}

		[Fact]
		public void DecrWithSuccess()
		{
			//first set intial value, step and expected result
			var key = "incr-success";
			long initial = 20;
			long step = -8;
			var result = initial + step;

			//call incrdecr first to set initial value, there's no step yet
			var actual = bucket.IncrDecr(key, initial, new TimeSpan(1, 0, 0));
			Assert.Equal(initial, actual);

			//now call incr/decr again with an 8 step to ensure you get the final result back.
			actual = bucket.IncrDecr(key, step);
			Assert.Equal(result, actual);

			bucket.Delete(key);
		}

		[Fact]
		public void IncrWithException()
		{
			var key = "incr-exception";
			var value = 5;

			var success = bucket.Set(key, value);
			Assert.True(success);
			Assert.Throws<IncrDecrNonNumericException>(() => bucket.IncrDecr(key, 8));
		}
	}
}
