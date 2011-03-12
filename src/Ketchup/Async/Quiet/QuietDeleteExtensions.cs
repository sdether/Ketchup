﻿using System;

namespace Ketchup.Async.Quiet {

	public static class QuietDeleteExtensions {
		public static KetchupClient Delete(this KetchupClient client, string key, Action<Exception> error) {
			return client.Delete(key, null, error);
		}
	}
}

