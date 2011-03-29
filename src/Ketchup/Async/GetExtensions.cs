using System;
using Ketchup.Protocol;
using Ketchup.Protocol.Commands;

namespace Ketchup.Async
{

	public static class GetExtensions
	{
		public static KetchupClient Get<T>(this KetchupClient client, string key, Action<T, object> hit, Action<object> miss, Action<Exception,object> error, object state) {
			return new GetCommand<T>(key)
			{
				Client = client,
				Error = error,
				Hit = hit,
				Key = key,
				Miss = miss,
				State = state
			}.Get();
		}
	}
}

