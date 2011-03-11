using System;
using Ketchup.Async;

namespace Ketchup.Async.Silent {

	public static class SilentSetExtensions {

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value) {
			return client.Set(key, value, null, null);
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value, TimeSpan expiration) {
			return client.Set(key, value, expiration, null, null);
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value, DateTime expiration) {
			return client.Set(key, value, expiration, null, null);
		}
	}
}

