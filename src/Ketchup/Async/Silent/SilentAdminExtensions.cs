namespace Ketchup.Async.Silent {

	public static class SilentAdminExtensions {

		public static KetchupClient Version(this KetchupClient client, string address) {
			return client.Version(address, null, null);
		}

		public static KetchupClient NoOp(this KetchupClient client, string address) {
			return client.NoOp(address, null, null);
		}

		public static KetchupClient Quit(this KetchupClient client, string address) {
			return client.Quit(address, null, null);
		}
	}
}

