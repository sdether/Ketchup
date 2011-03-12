using System;
using Ketchup.Protocol;
using Ketchup.Async;

namespace Ketchup.Async.Silent {

	public static class SilentGetExtensions {
		public static KetchupClient Get<T>(this KetchupClient client, string key, Action<T> hit) {
			return client.Get<T>(key, hit, null, null);
		}
	}
}

