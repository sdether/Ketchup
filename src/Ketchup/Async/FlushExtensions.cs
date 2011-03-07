using System;
using Ketchup.Protocol;
using Ketchup.Config;

namespace Ketchup.Async {

	public static class FlushExtensions {
		public static KetchupClient Flush(this KetchupClient client, string host, string port, 
			Action success, Action<Exception> error) {
			return Flush(client, host + ":" + port, success, error);
		}

		public static KetchupClient Flush(this KetchupClient client, string address,
			Action success, Action<Exception> error) {

			var node = KetchupConfig.Current.GetNode(address);
			if (node == null)
				throw new Exception("Host and port specified are not a valid node in the Ketchup Config");

			Operations.Flush(Op.Flush, node, 0, success, error);
			return client;
		}



	}
}

