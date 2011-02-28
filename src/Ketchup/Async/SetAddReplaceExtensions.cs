using System;
using Ketchup.Protocol;

namespace Ketchup.Async {

	public static class SetAddReplaceExtensions {
		/// <summary>
		/// If no success action is specified, set the key/value quietly, no response expected.
		/// </summary>
		/// <typeparam name="T">type of the Value</typeparam>
		/// <param name="client">Ketchup Client instance</param>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		/// <param name="success">set</param>
		/// <param name="error">error callback, invoked if memcached returns an error</param>
		/// <returns></returns>
		//I have to think more about what it means to set quietly in an async world
		//public static KetchupClient Set<T>(this KetchupClient client, string key, T value, Action<Exception> error) {
		//    return Set<T>(client, key, value, DateTime.MinValue, error);
		//}

		//public static KetchupClient Set<T>(this KetchupClient client, string key, T value, DateTime expiration, Action<Exception> error) {
		//    Operations.Set<T>(Op.SetQ, key, value, expiration, client.Bucket, () => {}, error);
		//    return client;
		//}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value, Action success, Action<Exception> error) {
			return Set(client, key, value, DateTime.MinValue, success, error);
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, 
			T value, TimeSpan expiration, Action success, Action<Exception> error) {
			//memcached treats timespans greater than 30 days as unix epoch time, convert to datetime
			if (expiration.TotalDays > 30)
				Set(client, key, value, DateTime.UtcNow + expiration, success, error);

			Operations.SetAddReplace(Op.Set, key, value, expiration.Seconds, client.Bucket, success, error);
			return client;
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, T value, DateTime expiration, Action success, Action<Exception> error) {
			var exp = (expiration - new DateTime(1970, 1, 1)).Seconds;
			Operations.SetAddReplace(Op.Set, key, value, exp, client.Bucket, success, error);
			return client;
		}

		public static KetchupClient Add<T>(this KetchupClient client, string key, T value, Action success, Action<Exception> error) {
			return Add(client, key, value, DateTime.MinValue, success, error);
		}

		public static KetchupClient Add<T>(this KetchupClient client, string key, T value, TimeSpan expiration, Action success, Action<Exception> error) {
			//memcached treats timespans greater than 30 days as unix epoch time, convert to datetime
			if (expiration.TotalDays > 30)
				Add(client, key, value, DateTime.UtcNow + expiration, success, error);

			Operations.SetAddReplace(Op.Add, key, value, expiration.Seconds, client.Bucket, success, error);
			return client;
		}

		public static KetchupClient Add<T>(this KetchupClient client, string key, T value, DateTime expiration, Action success, Action<Exception> error) {
			var exp = (expiration - new DateTime(1970, 1, 1)).Seconds;
			Operations.SetAddReplace(Op.Add, key, value, exp, client.Bucket, success, error);
			return client;
		}

		public static KetchupClient Replace<T>(this KetchupClient client, string key, T value, Action success, Action<Exception> error) {
			return Replace(client, key, value, DateTime.MinValue, success, error);
		}

		public static KetchupClient Replace<T>(this KetchupClient client, string key, T value, TimeSpan expiration, Action success, Action<Exception> error) {
			//memcached treats timespans greater than 30 days as unix epoch time, convert to datetime
			if (expiration.TotalDays > 30)
				Replace(client, key, value, DateTime.UtcNow + expiration, success, error);

			Operations.SetAddReplace(Op.Replace, key, value, expiration.Seconds, client.Bucket, success, error);
			return client;
		}

		public static KetchupClient Replace<T>(this KetchupClient client, string key, T value, DateTime expiration, Action success, Action<Exception> error) {
			var exp = (expiration - new DateTime(1970, 1, 1)).Seconds;
			Operations.SetAddReplace(Op.Replace, key, value, exp, client.Bucket, success, error);
			return client;
		}

	}
}

