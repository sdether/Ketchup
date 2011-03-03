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
		private const string numValue = "20";
		private const long longValue = 1;
		private readonly byte[] byteValue = new byte[2000000];


		[Fact]
		public string SetWithSuccess() {
			//have to do this for async operations in unit test framework
			var block = true;
			Exception exb = null;
			var cli = new KetchupClient(BuildConfiguration(), "default");

			cli.Set(
				key,
				stringValue,
				() => {
					block = false;
					Assert.True(true);
				},
				ex => {
					block = false;
					exb = ex;
					throw ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 100) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;

			return stringValue;
		}

		[Fact]
		public long SetLongWithSuccess() {
			//have to do this for async operations in unit test framework
			Exception exb = null;
			var block = true;
			var cli = new KetchupClient(BuildConfiguration(), "default");

			cli.Set(key,longValue,
				() => {
					block = false;
					Assert.True(true);
				},
				ex => {
					block = false;
					exb = ex;
					throw ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 100) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;

			return longValue;
		}

		[Fact]
		public string SetNumericWithSuccess() {
			//have to do this for async operations in unit test framework
			Exception exb = null;
			var block = true;
			var cli = new KetchupClient(BuildConfiguration(), "default");

			cli.Set(key, numValue,
				() => {
					block = false;
					Assert.True(true);
				},
				ex => {
					block = false;
					exb = ex;
					throw ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 100) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;

			return numValue;
		}


		[Fact]
		public void SetWithException() {
			//have to do this for async operations in unit test framework
			var block = true;
			Exception exb = null;
			ProtocolException exp = null;

			//I should get value too large
			var cli = new KetchupClient(BuildConfiguration(), "default");

			cli.Set(key,byteValue,
				() => {
					block = false;
					exb = new Exception("Success was fired, but should be exception");
				},
				ex => {
					block = false;
					exp = ex as ValueTooLargeException;
					if (exp == null)
						exb = ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 100) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;

			Assert.NotNull(exp);
		}

		[Fact]
		public void ReplaceWithException() {
			//have to do this for async operations in unit test framework
			var block = true;
			Exception exb = null;
			ProtocolException exp = null;

			var cli = new KetchupClient(BuildConfiguration(), "default");
			cli.Replace(new Random().Next().ToString(),stringValue,
				() => {
					block = false;
					exb = new Exception("Success was fired, but should be exception");
				},
				ex => {
					block = false;
					exp = ex as NotFoundException;
					if (exp == null)
						exb = ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 100) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;

			Assert.NotNull(exp);
		}

		[Fact]
		public void AddWithException() {
			//have to do this for async operations in unit test framework
			var block = true;
			Exception exb = null;
			ProtocolException exp = null;

			var value = SetWithSuccess();
			var cli = new KetchupClient(BuildConfiguration(), "default");

			cli.Add(key, value,
				() => {
					block = false;
					exb = new Exception("Success was fired, but should be exception");
				},
				ex => {
					block = false;
					exp = ex as KeyExistsException;
					if (exp == null)
						exb = ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 100) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;

			Assert.NotNull(exp);
		}

		[Fact]
		public void SetWithExpirationTest() {

		}

		[Fact]
		public long GetLong() {
			var block = true;
			Exception exb = null;
			long returnValue = 1;
			var cli = new KetchupClient(BuildConfiguration(), "default");
			cli.Get<long>(key,
				//hit
				val => {
					block = false;
					returnValue = val;
				},
				//miss
				() => {
					block = false;
					exb = new Exception("A miss was fired");
				},
				//error
				ex => {
					block = false;
					exb = ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 6) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;

			Assert.Equal(longValue, returnValue);
			return returnValue;
		}

		[Fact]
		public string GetWithHit() {
			var block = true;
			Exception exb = null;
			var returnValue = "";

			var value = SetWithSuccess();
			var cli = new KetchupClient(BuildConfiguration(), "default");
			cli.Get<string>(key,
				//hit
				val => {
					block = false;
					returnValue = val;
				},
				//miss
				() => {
					block = false;
					exb = new Exception("A miss was fired");
				},
				//error
				ex => {
					block = false;
					exb = ex;
					throw ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 6) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;

			Assert.Equal(value, returnValue);
			return returnValue;
		}

		[Fact]
		public int GetIntWithHit() {
			var block = true;
			Exception exb = null;
			var value = SetLongWithSuccess();
			var returnValue = -1;

			var cli = new KetchupClient(BuildConfiguration(), "default");
			cli.Get<int>(key,
				//hit
				val => {
					block = false;
					returnValue = val;
				},
				//miss
				() => {
					block = false;
					exb = new Exception("A miss was fired");
				},
				//error
				ex => {
					block = false;
					exb = ex;
					throw ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 6) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;

			Assert.Equal(value, returnValue);
			return returnValue;
		}


		[Fact]
		public static void GetWithMiss() {
			var block = true;
			Exception exb = null;
			var cli = new KetchupClient(BuildConfiguration(), "default");
			cli.Get<string>(new Random().Next().ToString(),
				//hit
				val => {
					block = false;
					exb = new Exception("Hit fired. Was expecting miss");
				},
				//miss
				() => {
					block = false;
					Assert.True(true);
				},
				//error
				ex => {
					block = false;
					exb = ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 6) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;
		}

		[Fact]
		public void DeleteWithSuccess() {
			Exception exb = null;
			var block = true;
			var value = SetWithSuccess();
			var cli = new KetchupClient(BuildConfiguration(), "default");
			cli.Delete(key,
				//success
				() => {
					block = false;
					value = "got here";
				},
				//error
				ex => {
					block = false;
					exb = ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 6) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;

			Assert.Equal(value, "got here");
		}

		[Fact]
		public void DeleteWithException() {
			//have to do this for async operations in unit test framework
			var block = true;
			Exception exb = null;
			ProtocolException exp = null;
			var cli = new KetchupClient(BuildConfiguration(), "default");

			cli.Delete(new Random().Next().ToString(),
				() => {
					block = false;
					exb = new Exception("Success was fired, but should be exception");
				},
				ex => {
					block = false;
					exp = ex as NotFoundException;
					if (exp == null)
						exb = ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 100) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;

			Assert.NotNull(exp);
		}

		//[Fact]
		//public void IncrWithInitialExpiration() {
		//    Exception exb = null;
		//    const long step = 1;
		//    var returnValue = '0';
		//    var block = true;
		//    var value = SetCharWithSuccess();
		//    var incrValue = longValue + step;
		//    var cli = new KetchupClient(BuildConfiguration(), "default");

		//    cli.IncrDecr(key,step,
		//        //success
		//        ul => {
		//            block = false;
		//            returnValue = ch;
		//        },
		//        //error
		//        ex => {
		//            block = false;
		//            exb = ex;
		//        });

		//    //have to block to wait for the async op to complete, otherwise method returns pass;
		//    var counter = 0;
		//    while (block) {
		//        if (++counter == 6) throw new TimeoutException("Operation has timed out with no response");
		//        Thread.Sleep(500);
		//    }

		//    if (exb != null)
		//        throw exb;

		//    Assert.Equal(returnValue, Convert.ToChar(incrValue));
		//}

		[Fact]
		public void IncrWithSuccess() {
			Exception exb = null;
			const long step = 100;
			ulong returnValue = 0;
			var block = true;
			var iv = ulong.Parse(SetNumericWithSuccess());
			var incrValue = iv + step;
			var cli = new KetchupClient(BuildConfiguration(), "default");

			cli.IncrDecr(key, step,
				//success
				ul => {
					block = false;
					returnValue = ul;
				},
				//error
				ex => {
					block = false;
					exb = ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			var counter = 0;
			while (block) {
				if (++counter == 6) throw new TimeoutException("Operation has timed out with no response");
				Thread.Sleep(500);
			}

			if (exb != null)
				throw exb;

			Assert.Equal(incrValue, returnValue);
		}

		[Fact]
		public void IncrWithException() {
		}

		private static KetchupConfig BuildConfiguration() {
			var kc = new KetchupConfig()
				.AddNode("DEVCACHE01:11211")
				.AddBucket();

			return kc;
		}
	}
}
