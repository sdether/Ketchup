using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Ketchup;
using Ketchup.Asynchronous;
using Ketchup.Config;
using System.Threading;

namespace Ketchup.Tests {
	public class AsyncTests {

		[Fact]
		public void SetWithSuccess() {
			//have to do this for async operations in unit test framework
			var block = true;

			var cli = new KetchupClient(BuildConfiguration(), "default");
			cli.Set<string>(
				"hello",
				"world!",
				() => {
					block = false;
					Assert.True(true);
				},
				ex => {
					block = false;
					throw ex;
				});

			//have to block to wait for the async op to complete, otherwise method returns pass;
			while (block) Thread.Sleep(500);
		}

		[Fact]
		public static void SetWithException() {

		}

		[Fact]
		public static void SetWithExpirationTest() {

		}

		[Fact]
		public static void GetWithMiss() {
			var cli = new KetchupClient(BuildConfiguration(), "default");
			cli.Get<string>(
				new Random().Next().ToString(),
				//hit
				val => {
					Assert.Null(val);
				},
				//miss
				() => {
					Assert.NotNull(1);
				},
				//error
				ex => {
					throw ex;
				});
		}

		private static KetchupConfig BuildConfiguration() {
			var kc = new KetchupConfig()
				.AddNode("DEVCACHE01:11240")
				.AddBucket();

			return kc;
		}
	}
}
