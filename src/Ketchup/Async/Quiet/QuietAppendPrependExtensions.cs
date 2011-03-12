using System;

namespace Ketchup.Async.Quiet {

	public static class QuietAppendPrependExtensions {

		public static KetchupClient Append(this KetchupClient client, string key, string value, Action<Exception> error) {
			return client.Append(key, value, null, error);
		}

		public static KetchupClient Prepend(this KetchupClient client, string key, string value, Action<Exception> error) {
			return client.Prepend(key, value, null, error);
		}
	}
}

