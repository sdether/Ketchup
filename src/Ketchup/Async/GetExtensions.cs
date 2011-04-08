using System;
using Ketchup.Protocol.Commands;

namespace Ketchup.Async
{

	public static class GetExtensions
	{
		public static KetchupClient Get<T>(this KetchupClient client, string key, string bucket, Action<T, object> hit, Action<object> miss, Action<Exception,object> error, object state) {
			return new GetCommand<T>(client, key, bucket)
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

