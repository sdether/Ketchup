using System;
using Xunit;
using System.Threading;
using Ketchup.Async;
using Ketchup.Config;
using Ketchup.Protocol.Exceptions;

namespace Ketchup.Tests {
	public class AsyncTests {
		private readonly byte[] byteValue = new byte[2000000];
		private static KetchupClient cli = new KetchupClient(TestHelpers.BuildConfiguration(), "default");

		[Fact]
		public void FlushWithSuccess() {
			var key = "flush-success";
			var value = key + "-value";
			var address = "DEVCACHE01:11211";

			TestHelpers.TestAsync((success, fail) => {
				cli.Set(key, value, success, ex => success());
			});

			TestHelpers.TestAsync((success, fail) => {
				cli.Flush(address, success, fail);
			});

			TestHelpers.TestAsync((success, fail) => {
				cli.Replace(key, value,
					() => fail(new Exception("Success was fired, but should be exception")),
					ex => { if (ex is NotFoundException) success(); else fail(ex); }
				);
			});
		}

		[Fact]
		public void SetWithSuccess() {
			var key = "set-success";
			var value = key + "-value";

			TestHelpers.TestAsync((success, fail) =>
				 cli.Set(key, value, success, fail)
			);
		}

		[Fact]
		public void SetWithException() {
			var key = "set-exception";
			TestHelpers.TestAsync((success, fail) =>
				cli.Set(key, byteValue,
				() => {
					fail(new Exception("Success was fired, but should be exception"));
				},
				ex => {
					if (ex is ValueTooLargeException)
						success();
					else
						fail(ex);
				})
			);
		}

		[Fact]
		public void ReplaceWithException() {
			var value = "replace-exception-value";
			TestHelpers.TestAsync((success, fail) =>
				cli.Replace(new Random().Next().ToString(), value,
					() => fail(new Exception("Success was fired, but should be exception")),
					ex => {
						if (ex is NotFoundException)
							success();
						else
							fail(ex);
					})
			);
		}

		[Fact]
		public void AddWithException() {
			var key = "add-exception";
			var value = key + "-value";

			//first set the value
			TestHelpers.TestAsync((success,fail) => {
				cli.Set(key, value, success, fail);
			});

			//then try to add a value with the same key
			TestHelpers.TestAsync((success, fail) =>
				cli.Add(key, value,
				() => fail(new Exception("Success was fired, but should be exception")),
				ex => {
					if (ex is KeyExistsException) success(); else fail(ex);
				})
			);
		}

		[Fact]
		public void SetWithExpirationTest() { }

		[Fact]
		public void GetWithHit() {
			var key = "get-hit";
			var value = key + "-value";

			//first set the value
			TestHelpers.TestAsync((success,fail) => {
				cli.Set(key,value, success, fail);
			});

			TestHelpers.TestAsync((success, fail) => {
				cli.Get<string>(key,
					hit: val => {
						Assert.Equal(value, val);
						success();
					},
					miss: () => fail(new Exception("A miss was fired")),
					error: fail
				);
			});
		}

		[Fact]
		public void GetWithMiss() {
			var key = "get-miss";

			//first delete the value
			TestHelpers.TestAsync((success, fail) => {
				//we don't actually care if it succeeds or fails, just go quietly
				cli.Delete(key, success, ex => success() );
			});

			TestHelpers.TestAsync((success, fail) =>
				cli.Get<string>(key,
					//hit
					val => fail(new Exception("Hit fired. Was expecting miss")),
					//miss
					success,
					//error
					fail
				)
			);
		}

		[Fact]
		public void DeleteWithSuccess() {
			var key = "delete-success";
			var value = key + "-value";

			//first set the value
			TestHelpers.TestAsync((success,fail) => {
				cli.Set(key,value);
				success();
			});

			TestHelpers.TestAsync((success, fail) =>
				cli.Delete(key, success, fail)
			);
		}

		[Fact]
		public void DeleteWithException() {
			TestHelpers.TestAsync((success, fail) =>
				cli.Delete(new Random().Next().ToString(),
					() => fail(new Exception("Success was fired, but should be exception")),
					ex => {
						if (ex is NotFoundException)
							success();
						else
							fail(ex);
					})
			);
		}

		[Fact]
		public void IncrWithSuccess() {
			var key = "incr-success";

			TestHelpers.TestAsync((success, fail) => {
				//first set intial value, step and expected result
				long initial = 20;
				long step = 8;
				var result = initial + step;
				//call incrdecr first to set initial value, there's no step yet
				cli.IncrDecr(key: key, initial: initial, expiration: new TimeSpan(1, 0, 0), success: v => {
					Assert.Equal(initial, v);
				}, error: fail)
				//now call incr/decr again with an 8 step to ensure you get the final result back.
				.IncrDecr(key: key, step: step, success: v1 => {
					Assert.Equal(result, v1);
					success();
				}, error: fail);
			});
		}

		[Fact]
		public void DecrWithSuccess() {
			var key = "decr-success";

			TestHelpers.TestAsync((success, fail) => {
				//first set intial value, step and expected result
				long initial = 20;
				long step = -8;
				var result = initial + step;
				//call incrdecr first to set initial value, there's no step yet
				cli.IncrDecr(key: key, initial: initial, expiration: new TimeSpan(1, 0, 0), success: v => {
					Assert.Equal(initial, v);
				}, error: fail)
				//now call incr/decr again with an 8 step to ensure you get teh final result back.
				.IncrDecr(key: key, step: step, success: v1 => {
					Assert.Equal(result, v1);
					success();
				}, error: fail);
			});
		}

		[Fact]
		public void IncrWithException() {
			var key = "incr-exception";

			TestHelpers.TestAsync((success, fail) => {
				cli.IncrDecr(key: key, step: 8, success: v1 =>
 					fail(new Exception("operation succeeded, but failure expected")),
					error:ex => {
						if (ex is IncrDecrNonNumericException)
							success();
						else
							fail(ex);
					});
				});
		}

		[Fact]
		public void AppendWithSuccess() {
			var key = "append-success";
			var value = key + "-value";
			var append = "-append";
			var expected = value + append;

			//first set the value
			TestHelpers.TestAsync((success,fail) => {
				cli.Set(key,value,success,fail);
			});

			TestHelpers.TestAsync((success, fail) => {
				cli.Append(key, append, success, fail);
			});

			TestHelpers.TestAsync((success, fail) => {
				cli.Get<string>(key, 
					//hit
					val => {
						Assert.Equal(expected, val);
						success();
					}, 
					//miss
					() => fail(new Exception("miss fired but hit was expected")),
					//error
					fail);
			});
		}

		[Fact]
		public void AppendWithException() {
			var key = "append-exception";
			var append = "-append";

			TestHelpers.TestAsync((success, fail) => {
				cli.Append(key, append, 
					success: () => fail(new Exception("success fired but exception was expected")),
					error:ex => { if (ex is ItemNotStoredException) success(); else fail(ex); }
				);
			});
		}

		[Fact]
		public void PrependWithSuccess() {
			var key = "prepend-success";
			var value = key + "-value";
			var prepend = "prepend-";
			var expected = prepend + value;

			//first set the value
			TestHelpers.TestAsync((success, fail) => {
				cli.Set(key, value, success, fail);
			});

			TestHelpers.TestAsync((success, fail) => {
				cli.Prepend(key, prepend, success, fail);
			});

			TestHelpers.TestAsync((success, fail) => {
				cli.Get<string>(key,
					hit:val => {
						Assert.Equal(expected, val);
						success();
					},
					miss:() => fail(new Exception("miss fired but hit was expected")),
					error:fail);
			});
		}

	}
}
