using System;
using Ketchup.Protocol;

namespace Ketchup.Async {

	public static class GetExtensions {
		public static KetchupClient Get<T>(this KetchupClient client, string key, Action<T> hit, Action miss, Action<Exception> error) {
			var op = miss == null ? Op.GetQ : Op.Get;
			Operations.Get(op, key, client.Bucket, hit, miss, error);
			return client;
		}
	}
}

