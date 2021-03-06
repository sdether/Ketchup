﻿using Ketchup.Sync;

namespace Ketchup.Commands
{
	public static class GetExtensions
	{
		public static T Get<T>(this Bucket bucket, string key)
		{
			return SyncGetExtensions.Get<T>(bucket, key);
		}
	}
}
