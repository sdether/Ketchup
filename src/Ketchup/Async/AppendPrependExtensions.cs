using System;
using Ketchup.Protocol;

namespace Ketchup.Async {

	public static class AppendPrependExtensions {

		public static KetchupClient Append(this KetchupClient client, string key,
			string value, Action success, Action<Exception> error) {

			var op = success == null ? Op.AppendQ : Op.Append;
			Operations.AppendPrepend(op, key, value, client.Bucket, success, error);
			return client;
		}

		public static KetchupClient Prepend(this KetchupClient client, string key,
			string value, Action success, Action<Exception> error) {
		
			var op = success == null ? Op.PrependQ : Op.Prepend;
			Operations.AppendPrepend(op, key, value, client.Bucket, success, error);
			return client;
		}
	}
}

