using System;

namespace Ketchup.Async.Silent {

	public static class IncrDecrExtensions {

		public static KetchupClient IncrDecr(this KetchupClient client, string key, long step) {
			return client.IncrDecr(key, step, null, null);
		}

		public static KetchupClient IncrDecr(this KetchupClient client, string key, long initial, TimeSpan expiration) {
			return client.IncrDecr(key, initial, expiration, null, null);
		}

		public static KetchupClient IncrDecr(this KetchupClient client, string key, long initial, DateTime expiration) {
			return client.IncrDecr(key, initial, expiration, null, null);
		}
	}
}

