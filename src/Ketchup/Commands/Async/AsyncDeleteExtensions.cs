using System;
using Ketchup.Protocol.Commands;

namespace Ketchup.Async
{
	public static class AsyncDeleteExtensions
	{
		public static Bucket Delete(this Bucket bucket, string key, 
			Action<object> success, Action<Exception, object> error, object state)
		{
			return new DeleteCommand(bucket, key)
			{
				State = state,
				Error = error,
				Success = success
			}.Delete();
		}
	}
}

