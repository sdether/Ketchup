using System;
using Ketchup.Protocol.Commands;

namespace Ketchup.Async
{
	public static class SetExtensions {
		public static KetchupClient Set<T>(this KetchupClient client, string key, string bucket, 
			T value, int expiration, Action<object> success, Action<Exception, object> error, object state) {

			return new SetAddReplaceCommand<T>(client, key, bucket, value) {
				Client = client,
				Key = key,
				Expiration = expiration,
				Success = success,
				Error = error,
				State = state
			}.Set();
		}

		public static KetchupClient Set<T>(this KetchupClient client, string key, string bucket, T value,
			Action<object> success, Action<Exception, object> error, object asyncState) {

			return Set(client, key, bucket, value, 0, success, error, asyncState);
		}

		//public static KetchupClient Set<T>(this KetchupClient client, string key, T value, TimeSpan expiration,
		//    Action success, Action<Exception> error)
		//{
		//    //memcached treats timespans greater than 30 days as unix epoch time, convert to datetime
		//    return expiration.TotalDays > 30 ?
		//        client.Set(key, value, DateTime.UtcNow + expiration, success, error) :
		//        client.Set(key, value, expiration.Seconds, success, error);
		//}

		//public static KetchupClient Set<T>(this KetchupClient client, string key, T value, DateTime expiration,
		//    Action success, Action<Exception> error)
		//{
		//    var exp = expiration == DateTime.MinValue ? 0 : (expiration - new DateTime(1970, 1, 1)).Seconds;
		//    return client.Set(key, value, exp, success, error);
		//}

	}
}

