using System;
using Ketchup.Protocol;

namespace Ketchup.Async {

	public static class AppendPrependExtensions {

		public static KetchupClient Append<T>(this KetchupClient client, string key,
			T value, Action success, Action<Exception> error) {

			Operations.AppendPrepend<T>(Op.Append, key, value, client.Bucket, success, error);
			return client;
		}

		public static KetchupClient Prepend<T>(this KetchupClient client, string key,
			T value, Action success, Action<Exception> error) {

			Operations.AppendPrepend<T>(Op.Prepend, key, value, client.Bucket, success, error);
			return client;
		}
	}
}

