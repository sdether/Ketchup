using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ketchup.Config;
using Ketchup.Async;
using System.Threading;

namespace Ketchup.Tests {
	public static class TestHelpers {
		public const string Address = "DEVCACHE01:11211";

		public static KetchupConfig BuildConfiguration() {
			var kc = new KetchupConfig()
				.AddNode(Address)
				.AddBucket();

			return kc;
		}

		public static void TestAsync(Action<Action, Action<Exception>> action) {
			var resetEvent = new ManualResetEvent(initialState: false);
			Exception exception = null;

			action(
				//delegate to invoke on test success
				() => {
					resetEvent.Set();
				},

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
