using System;
using Ketchup.Protocol;

namespace Ketchup.Async {

	public static class DeleteExtensions {

		public static KetchupClient Delete(this KetchupClient client, string key, Action success, Action<Exception> error) {
			Operations.Delete(Op.Delete, key, client.Bucket, success, error);
			return client;
		}
	}
}

