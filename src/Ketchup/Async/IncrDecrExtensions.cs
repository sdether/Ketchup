using System;
using Ketchup.Protocol;

namespace Ketchup.Async {

	public static class IncrDecrExtensions {

		public static KetchupClient IncrDecr(this KetchupClient client, string key, 
			long initial, long step, int expiration, Action<long> success, Action<Exception> error) {

			var op = step < 0 ? 
				(success == null ? Op.DecrQ : Op.Decr): 
				(success == null ? Op.IncrQ : Op.Incr);

			step = Math.Abs(step);
			Operations.IncrDecr(op, key, initial, step, expiration, client.Bucket, success, error);
			return client;
		}

		public static KetchupClient IncrDecr(this KetchupClient client, 
			string key, long step, Action<long> success, Action<Exception> error) {
			return client.IncrDecr(key, 0, step, 0, success, error);
		}

		public static KetchupClient IncrDecr(this KetchupClient client,
			string key, long initial, TimeSpan expiration,
			Action<long> success, Action<Exception> error) {

			//memcached treats timespans greater than 30 days as unix epoch time, convert to datetime
			return expiration.TotalDays > 30 ?
				client.IncrDecr(key, initial, DateTime.UtcNow + expiration, success, error) :
				client.IncrDecr(key, initial, 0, expiration.Seconds, success, error);
		}

		public static KetchupClient IncrDecr(this KetchupClient client,
			string key, long initial, DateTime expiration,
			Action<long> success, Action<Exception> error) {

			//expiration in datetime converted to epoch
			var exp = expiration == DateTime.MinValue ? 0 : (expiration - new DateTime(1970, 1, 1)).Seconds;
			return client.IncrDecr(key, initial, 0, exp, success, error);
		}

	}
}

