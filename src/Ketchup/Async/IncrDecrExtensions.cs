using System;
using Ketchup.Protocol;

namespace Ketchup.Async {

	public static class IncrDecrExtensions {

		public static KetchupClient IncrDecr(this KetchupClient client, 
			string key, long step, Action<long> success, Action<Exception> error) {

			var op = step < 0 ? Op.Decr : Op.Incr;
			step = Math.Abs(step);
			Operations.IncrDecr(op, key, 0, step, 0, client.Bucket, success, error);

			return client;
		}

		public static KetchupClient IncrDecr(this KetchupClient client,
			string key, long initial, TimeSpan expiration,
			Action<long> success, Action<Exception> error) {

			if (expiration.TotalDays > 30)
				IncrDecr(client, key, initial, DateTime.UtcNow + expiration, success, error);

			Operations.IncrDecr(Op.Incr, key, initial, 0, expiration.Seconds, client.Bucket, success, error);
			return client;
		}

		public static KetchupClient IncrDecr(this KetchupClient client,
			string key, long initial, DateTime expiration,
			Action<long> success, Action<Exception> error) {

			//expiration in datetime converted to epoch
			var exp = (expiration - new DateTime(1970, 1, 1)).Seconds;

			Operations.IncrDecr(Op.Incr, key, initial, 0, exp, client.Bucket, success, error);
			return client;
		}

	}
}

