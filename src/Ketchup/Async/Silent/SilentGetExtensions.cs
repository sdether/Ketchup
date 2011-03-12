using System;

namespace Ketchup.Async.Silent {

	public static class SilentGetExtensions {
		public static KetchupClient Get<T>(this KetchupClient client, string key, Action<T> hit) {
			return client.Get(key, hit, null, null);
		}
	}
}

