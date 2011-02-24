using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ketchup.Protocol;
using Ketchup.Config;
using System.Threading;
using Ketchup.Protocol;

namespace Ketchup.Asynchronous {

	public static class GetExtensions {
		private static KetchupConfig config = KetchupConfig.Current;

		public static KetchupClient Get<T>(this KetchupClient client, string key, Action<T> hit, Action miss, Action<Exception> error) {
			Operations.Get<T>(Op.Get, key, client.Bucket, hit, miss, error);
			return client;
		}

		public static KetchupClient GetQ<T>(this KetchupClient client, string key,
			Action<T> hit, Action miss, Action<Exception> error) {
			Operations.Get<T>(Op.GetQ, key, client.Bucket, hit, miss, error);
			return client;
		}
	}
}

