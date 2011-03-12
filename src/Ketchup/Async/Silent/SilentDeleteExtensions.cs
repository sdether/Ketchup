namespace Ketchup.Async.Silent {

	public static class SilentDeleteExtensions {
		public static KetchupClient Delete(this KetchupClient client, string key) {
			return client.Delete(key, null, null);
		}
	}
}

