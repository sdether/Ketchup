using System;
using Ketchup.Protocol;

namespace Ketchup.Async {

	public static class GetExtensions {
		public static KetchupClient Get<T>(this KetchupClient client, string key, Action<T> hit, Action miss, Action<Exception> error) {
			Operations.Get(Op.GetK, key, client.Bucket, hit, miss, error);
			return client;
		}

		//what does GetQ mean in a callback world?
		//public static KetchupClient GetQ<T>(this KetchupClient client, string key,
		//    Action<T> hit, Action miss, Action<Exception> error) {
		//    Operations.Get(Op.GetQ, key, client.Bucket, hit, miss, error);
		//    return client;
		//}
	}
}

