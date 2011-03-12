using System;
using Ketchup.Protocol;
using Ketchup.Config;

namespace Ketchup.Async {

	public static class FlushExtensions {
		
		public static KetchupClient Flush(this KetchupClient client, string address, int expiration,
			Action success, Action<Exception> error) {

			var node = KetchupConfig.Current.GetNode(address);
			var op = success == null ? Op.FlushQ : Op.Flush;
			Operations.Flush(op, node, expiration, success, error);
			return client;
		}

		public static KetchupClient Flush(this KetchupClient client, string address, 
			Action success, Action<Exception> error) {
			return client.Flush(address, 0, success, error);
		}

		public static KetchupClient Flush(this KetchupClient client, string address, TimeSpan expiration,
			Action success, Action<Exception> error) {
			//memcached treats timespans greater than 30 days as unix epoch time, convert to datetime
			return expiration.TotalDays > 30 ?
				client.Flush(address, DateTime.UtcNow + expiration, success, error) :
				client.Flush(address, expiration.Seconds, success, error);
		}

		public static KetchupClient Flush(this KetchupClient client, string address, DateTime expiration,
			Action success, Action<Exception> error) {
			var exp = expiration == DateTime.MinValue ? 0 : (expiration - new DateTime(1970, 1, 1)).Seconds;
			return client.Flush(address, exp, success, error);
		}
	}
}

