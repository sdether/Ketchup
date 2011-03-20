using System;
using System.Threading;
using Ketchup.Config;

namespace Ketchup.Tests {
	public static class TestHelpers {
		public const string Address = "DEVCACHE01:11211";

		public static KetchupConfig BuildConfiguration() {
			var kc = new KetchupConfig()
				.AddNode(Address)
				.AddBucket();

			return kc;
		}

		public static void TestAsync(Action<Action<string>, Action<string, Exception>> action) {
			var resetEvent = new ManualResetEvent(initialState: false);
			Exception exception = null;

			action(
				//delegate to invoke on test success
				(key) => {
					resetEvent.Set();
				},

				//delegate to invoke on test error
				(key, e) => {
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
