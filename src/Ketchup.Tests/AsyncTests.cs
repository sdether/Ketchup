using System;
using Xunit;
using System.Threading;
using Ketchup.Async;
using Ketchup.Config;
using Ketchup.Protocol.Exceptions;

namespace Ketchup.Tests {
	public class AsyncTests {

		private const string key = "hello";
		private const string stringValue = "world!";
		private readonly byte[] byteValue = new byte[2000000];
		private readonly KetchupClient cli = new KetchupClient(BuildConfiguration(), "default");

		[Fact]
		public void SetWithSuccess() {
			TestAsync((success, fail) =>
				 cli.Set(key, stringValue, success, fail)
			);
		}

		[Fact]
		public void SetWithException() {
			TestAsync((success, fail) =>
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
			TestAsync((success, fail) =>
				cli.Replace(new Random().Next().ToString(), stringValue,
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
			SetWithSuccess();
			TestAsync((success, fail) =>
				cli.Add(key, stringValue,
				() => fail(new Exception("Success was fired, but should be exception")),
				ex => {
					if (ex is KeyExistsException)
						success();
					else
						fail(ex);
				})
			);
		}

		[Fact]
		public void SetWithExpirationTest() { }

		[Fact]
		public void GetWithHit() {
			SetWithSuccess();
			TestAsync((success, fail) =>
			cli.Get<string>(key,
				//hit
				val => {
					Assert.Equal(stringValue, val);
					success();
				},
				//miss
				() => fail(new Exception("A miss was fired")),
				//error
				fail
				)
			);
		}

		[Fact]
		public void GetWithMiss() {
			TestAsync((success, fail) =>
				cli.Get<string>(new Random().Next().ToString(),
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
			SetWithSuccess();
			TestAsync((success, fail) =>
				cli.Delete(key, success, fail)
			);
		}

		[Fact]
		public void DeleteWithException() {
			TestAsync((success, fail) =>
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
			DeleteWithSuccess();
			TestAsync((success, fail) => {
				//first set intial value, step and expected result
				long initial = 20;
				long step = 8;
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
		public void IncrWithNegative() {
			DeleteWithSuccess();
			TestAsync((success, fail) => {
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
			SetWithSuccess();
			TestAsync((success, fail) => {
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

		private static KetchupConfig BuildConfiguration() {
			var kc = new KetchupConfig()
				.AddNode("DEVCACHE01:11211")
				.AddBucket();

			return kc;
		}

		private static void TestAsync(Action<Action, Action<Exception>> action) {
			var resetEvent = new ManualResetEvent(initialState: false);
			Exception exception = null;

			action(
				//delegate to invoke on test success
				() => resetEvent.Set(),

				//delegate to invoke on test error
				e => {
					exception = e;
					resetEvent.Set();
				});

			if (!resetEvent.WaitOne(30 * 1000))
				throw new TimeoutException("Operation timed out before receiving a WaitHandle.Set();");

			if (exception != null)
				throw exception;

		}
	}
}
