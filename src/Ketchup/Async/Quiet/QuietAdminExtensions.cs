using System;
using Ketchup.Protocol;
using Ketchup.Config;

namespace Ketchup.Async.Quiet {

	public static class QuietAdminExtensions {

		public static KetchupClient Version(this KetchupClient client, string address, Action<Exception> error) {
			return client.Version(address, null, error);
		}

		public static KetchupClient NoOp(this KetchupClient client, string address, Action<Exception> error) {
			return client.NoOp(address, null, error);
		}

		public static KetchupClient Quit(this KetchupClient client, string address, Action<Exception> error) {
			return client.Quit(address, null, error);
		}
	}
}

