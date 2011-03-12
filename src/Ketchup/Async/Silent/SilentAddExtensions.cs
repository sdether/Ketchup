using System;

namespace Ketchup.Async.Silent {

	public static class SilentAddExtensions {

		public static KetchupClient Add<T>(this KetchupClient client, string key, T value) {
			return client.Add(key, value, null, null);
		}

		public static KetchupClient Add<T>(this KetchupClient client, string key, T value, TimeSpan expiration) {
			return client.Add(key, value, expiration, null, null);
		}

		public static KetchupClient Add<T>(this KetchupClient client, string key, T value, DateTime expiration) {
			return client.Add(key, value, expiration, null, null);
		}
	}
}

