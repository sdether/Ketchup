using System;
using Ketchup.Protocol.Commands;

namespace Ketchup.Async
{
	public static class AsyncGetExtensions
	{
		public static Bucket Get<T>(this Bucket bucket, string key, Action<T, object> hit, Action<object> miss, Action<Exception,object> error, object state) {
			return new GetCommand<T>(bucket, key)
			{
				Error = error,
				Hit = hit,
				Key = key,
				Miss = miss,
				State = state
			}.Get();
		}
	}
}

