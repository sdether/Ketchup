using System;
using Ketchup.Config;
using Ketchup.Protocol.Commands;

namespace Ketchup.Async
{
	public static class AsyncAddExtensions
	{
		public static Bucket Add<T>(this Bucket bucket, string key, 
			T value, int expiration, 
			Action<object> success, Action<Exception, object> error, object state) {

			return new SetAddReplaceCommand<T>(bucket, key, value) {
				Key = key,
				Expiration = expiration,
				Success = success,
				Error = error,
				State = state
			}.Add();
		}

		public static Bucket Add<T>(this Bucket bucket, string key, 
			T value,
			Action<object> success, Action<Exception, object> error, object state) 
		{
			return Add(bucket, key, value, KetchupConfig.Current.DefaultExpiration, success, error, state);
		}

		public static Bucket Add<T>(this Bucket bucket, string key, 
			T value, TimeSpan expiration,
			Action<object> success, Action<Exception, object> error, object state) 
		{
			//memcached treats timespans greater than 30 days as unix epoch time, convert to datetime
			return expiration.TotalDays > 30 ?
				Add(bucket, key, value, DateTime.UtcNow + expiration, success, error, state) :
				Add(bucket, key, value, Convert.ToInt32(expiration.TotalSeconds), success, error, state);
		}

		public static Bucket Add<T>(this Bucket bucket, string key, 
			T value, DateTime expiration,
			Action<object> success, Action<Exception, object> error, object state) 
		{
			var exp = expiration == DateTime.MinValue ? 0 : Convert.ToInt32((expiration - new DateTime(1970, 1, 1)).TotalSeconds);
			return Add(bucket, key, value, exp, success, error, state);
		}

	}
}

