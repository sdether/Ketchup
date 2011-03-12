namespace Ketchup.Async.Silent {

	public static class SilentAppendPrependExtensions {

		public static KetchupClient Append(this KetchupClient client, string key, string value) {
			return client.Append(key, value, null, null);
		}

		public static KetchupClient Prepend(this KetchupClient client, string key, string value) {
			return client.Prepend(key, value, null, null);
		}
	}
}

