using System;

namespace Ketchup.Async.Silent {

	public static class SilentReplaceExtensions {

		public static KetchupClient Replace<T>(this KetchupClient client, string key, T value) {
			return client.Replace(key, value, null, null);
		}

		public static KetchupClient Replace<T>(this KetchupClient client, string key, T value, TimeSpan expiration) {
			return client.Replace(key, value, expiration, null, null);
		}

		public static KetchupClient Replace<T>(this KetchupClient client, string key, T value, DateTime expiration) {
			return client.Replace(key, value, expiration, null, null);
		}
	}
}

