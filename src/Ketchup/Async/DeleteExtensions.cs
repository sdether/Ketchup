using System;
using Ketchup.Protocol;

namespace Ketchup.Async {

	public static class DeleteExtensions {

		public static KetchupClient Delete(this KetchupClient client, string key, Action success, Action<Exception> error) {
			var op = success == null ? Op.DeleteQ : Op.Delete;

			Operations.Delete(op, key, client.Bucket, success, error);
			return client;
		}
	}
}

