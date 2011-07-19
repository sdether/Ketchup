using System;
using System.Threading;
using Ketchup.Config;

namespace Ketchup
{
	public static class SyncExtensions
	{
		public static void ExecuteSync(string key, string command, Action<Action<object>, Action<Exception, object>> action)
		{
			var resetEvent = new ManualResetEvent(initialState: false);
			Exception exception = null;

			action(
				//delegate to invoke on success
				s => resetEvent.Set(),

				//delegate to invoke on test error
				(e, s) =>
				{
					exception = e;
					resetEvent.Set();
				});

			var timeout = KetchupConfig.Current.SyncCommandTimeout;
			if (!resetEvent.WaitOne(timeout * 1000))
				throw new TimeoutException(
				string.Format(
						"Command timed out on before async response was returned.\nCommand: {0}\nKey: {1}",
						command,
						key
					));
			if (exception != null)
				throw exception;
		}

	}
}
