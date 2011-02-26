using System;
using Ketchup.Protocol;

namespace Ketchup.Asynchronous {

	public static class GetExtensions {
		public static KetchupClient Get<T>(this KetchupClient client, string key, Action<T> hit, Action miss, Action<Exception> error) {
			Operations.Get(Op.Get, key, client.Bucket, hit, miss, error);
			return client;
		}

		public static KetchupClient GetQ<T>(this KetchupClient client, string key,
			Action<T> hit, Action miss, Action<Exception> error) {
			Operations.Get(Op.GetQ, key, client.Bucket, hit, miss, error);
			return client;
		}
	}
}

