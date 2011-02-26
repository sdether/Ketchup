using System;
using Ketchup.Protocol;
using Xunit;
using Ketchup.Asynchronous;
using Ketchup.Config;
using System.Threading;

namespace Ketchup.Tests {
	public class AsyncTests {

		[Fact]
		public string SetWithSuccess() {
			//have to do this for async operations in unit test framework
			var block = true;
			Exception exb = null;
			const string value = "world!";
			var cli = new KetchupClient(BuildConfiguration(), "default");

			cli.Set(
				"hello",
				value,
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

			return value;
		}

		[Fact]
		public void SetWithException() {
			//have to do this for async operations in unit test framework
			var block = true;
			Exception exb = null;
			ProtocolException exp = null;

			//I should get value too large

			var value = new byte[2000000];

			var cli = new KetchupClient(BuildConfiguration(), "default");

			cli.Set(
				"giant byte",
				value,
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
		public void SetWithExpirationTest() {

		}

		[Fact]
		public void GetWithHit() {
			var block = true;
			Exception exb = null;
			var returnValue = "";

			var value = SetWithSuccess();

			var cli = new KetchupClient(BuildConfiguration(), "default");
			cli.Get<string>(
				"hello",
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
		}

		[Fact]
		public static void GetWithMiss() {
			var block = true;
			Exception exb = null;

			var cli = new KetchupClient(BuildConfiguration(), "default");
			cli.Get<string>(
				new Random().Next().ToString(),
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

		private static KetchupConfig BuildConfiguration() {
			var kc = new KetchupConfig()
				.AddNode("DEVCACHE01:11211")
				.AddBucket();

			return kc;
		}
	}
}
