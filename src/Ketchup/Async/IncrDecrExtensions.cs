using System;
using Ketchup.Protocol;

namespace Ketchup.Async {

	public static class IncrDecrExtensions {

		public static KetchupClient IncrDecr(this KetchupClient client, 
			string key, long step, Action<ulong> success, Action<Exception> error) {
			return IncrDecr(client, key, step, 0, DateTime.MinValue, success, error);
		}

		public static KetchupClient IncrDecr(this KetchupClient client,
			string key, long step, long initial, TimeSpan expiration,
			Action<ulong> success, Action<Exception> error) {

			if (expiration.TotalDays > 30)
				IncrDecr(client, key, step, initial, DateTime.UtcNow + expiration, success, error);

			var op = step >= 0 ? Op.Incr : Op.Decr;
			step = (int)Math.Abs(step);

			Operations.IncrDecr(op, key, step, initial, expiration.Seconds, client.Bucket, success, error);
			return client;
		}

		public static KetchupClient IncrDecr(this KetchupClient client, 
			string key, long step, long initial, DateTime expiration,
			Action<ulong> success, Action<Exception> error) {

			var exp = (expiration - new DateTime(1970, 1, 1)).Seconds;

			var op = step >= 0 ? Op.Incr : Op.Decr;
			step = (int)Math.Abs(step);
			
			Operations.IncrDecr(op, key, step, initial, exp, client.Bucket, success, error);
			return client;
		}

	}
}

