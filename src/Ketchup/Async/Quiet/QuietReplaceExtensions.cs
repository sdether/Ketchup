using System;
using Ketchup.Async;

namespace Ketchup.Async.Quiet {

	public static class QuietReplaceExtensions {

		public static KetchupClient Replace<T>(this KetchupClient client, string key, T value, Action<Exception> error) {
			return client.Replace(key, value, null, error);
		}

		public static KetchupClient Replace<T>(this KetchupClient client, string key, T value, TimeSpan expiration, Action<Exception> error) {
			return client.Replace(key, value, expiration, null, error);
		}

		public static KetchupClient Replace<T>(this KetchupClient client, string key, T value, DateTime expiration, Action<Exception> error) {
			return client.Replace(key, value, expiration, null, error);
		}
	}
}

