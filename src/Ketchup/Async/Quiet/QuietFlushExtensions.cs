using System;

namespace Ketchup.Async.Quiet {

	public static class QuietFlushExtensions {

		public static KetchupClient Flush(this KetchupClient client, string address, Action<Exception> error) {
			return client.Flush(address, null, error);
		}

		public static KetchupClient Flush(this KetchupClient client, string address, TimeSpan expiration, Action<Exception> error) {
			return client.Flush(address, expiration, null, error);
		}

		public static KetchupClient Flush(this KetchupClient client, string address, DateTime expiration, Action<Exception> error) {
			return client.Flush(address, expiration, null, error);
		}
	}
}

