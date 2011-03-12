using System;

namespace Ketchup.Async.Silent {

	public static class SilentFlushExtensions {

		public static KetchupClient Flush(this KetchupClient client, string address) {
			return client.Flush(address, null, null);
		}

		public static KetchupClient Flush(this KetchupClient client, string address, TimeSpan expiration) {
			return client.Flush(address, expiration, null, null);
		}

		public static KetchupClient Flush(this KetchupClient client, string address, DateTime expiration) {
			return client.Flush(address, expiration, null, null);
		}
	}
}

