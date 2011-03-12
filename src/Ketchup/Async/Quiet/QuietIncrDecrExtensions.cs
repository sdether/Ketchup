using System;
using Ketchup.Protocol;

namespace Ketchup.Async.Quiet {

	public static class IncrDecrExtensions {

		public static KetchupClient IncrDecr(this KetchupClient client, string key, long step, Action<Exception> error) {
			return client.IncrDecr(key, step, null, error);
		}

		public static KetchupClient IncrDecr(this KetchupClient client, string key, long initial, TimeSpan expiration, Action<Exception> error) {
			return client.IncrDecr(key, initial, expiration, null, error);
		}

		public static KetchupClient IncrDecr(this KetchupClient client, string key, long initial, DateTime expiration, Action<Exception> error) {
			return client.IncrDecr(key, initial, expiration, null, error);
		}

	}
}

