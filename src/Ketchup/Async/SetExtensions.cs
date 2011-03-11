using System;
using Ketchup.Protocol;

namespace Ketchup.Async {

	public static class SetExtensions {
		private static KetchupClient Set<T>(this KetchupClient client, string key, T value, int expiration,
			Action success, Action<Exception> error) {

			var op = success == null ? Op.SetQ : Op.Set;
			Operations.SetAddReplace(op, key, value, expiration, client.Bucket, success, error);
			return client;
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value,
			Action success, Action<Exception> error) {
			return Set(client, key, value, 0, success, error);
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value, TimeSpan expiration,
			Action success, Action<Exception> error) {
			//memcached treats timespans greater than 30 days as unix epoch time, convert to datetime
			return expiration.TotalDays > 30 ?
				Set(client, key, value, DateTime.UtcNow + expiration, success, error) :
				Set(client, key, value, expiration.Seconds, success, error);
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value, DateTime expiration,
			Action success, Action<Exception> error) {
			var exp = expiration == DateTime.MinValue ? 0 : (expiration - new DateTime(1970, 1, 1)).Seconds;
			return Set(client, key, value, exp, success, error);
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value, Action<Exception> error) {
			return Set(client, key, value, null, error);
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value, TimeSpan expiration, Action<Exception> error) {
			return Set(client, key, value, expiration, null, error);
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value, DateTime expiration, Action<Exception> error) {
			return Set(client, key, value, expiration, null, error);
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value) {
			return Set(client, key, value, null, null);
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value, TimeSpan expiration) {
			return Set(client, key, value, expiration, null, null);
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value, DateTime expiration) {
			return Set(client, key, value, expiration, null, null);
		}
	}
}

