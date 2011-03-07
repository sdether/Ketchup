using System;
using Ketchup.Protocol;

namespace Ketchup.Async {

	public static class AppendPrependExtensions {

		public static KetchupClient Append(this KetchupClient client, string key,
			string value, Action success, Action<Exception> error) {

			Operations.AppendPrepend(Op.Append, key, value, client.Bucket, success, error);
			return client;
		}

		public static KetchupClient Prepend(this KetchupClient client, string key,
			string value, Action success, Action<Exception> error) {

			Operations.AppendPrepend(Op.Prepend, key, value, client.Bucket, success, error);
			return client;
		}
	}
}

