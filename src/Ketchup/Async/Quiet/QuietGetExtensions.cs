using System;

namespace Ketchup.Async.Quiet {

	public static class QuietGetExtensions {
		public static KetchupClient Get<T>(this KetchupClient client, string key, Action<T> hit, Action<Exception> error) {
			return client.Get(key, hit, null, error);
		}
	}
}

