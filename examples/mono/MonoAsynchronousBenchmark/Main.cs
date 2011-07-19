using System;
using System.Threading;
using Ketchup.Async;
using Ketchup.Sync;

namespace Ketchup.Demo
{
	public class Program
	{
		private static bool debugAsync = true;

		static void Main(string[] args)
		{
			Run();
		}

		private static void Run()
		{
			Console.WriteLine("Enter the number of operations or enter to quit");
			var numberOfOperations = 0;
			if (!int.TryParse(Console.ReadLine(), out numberOfOperations)) return;

			//KetchupClient DefaultBucket defaults to "default" bucket name or first bucket in the list
			var bucket = new KetchupClient("localhost",11211).DefaultBucket;
			var ktimes = 0d;
			Console.WriteLine("Number of Operations: " + numberOfOperations);
			if (!debugAsync)
			{
				ktimes = SetAndGetKetchupSync(numberOfOperations, bucket);
				Console.WriteLine("Ketchup Sync: " + ktimes + " seconds");
			}

			var ktimea = SetAndGetKetchupAsync(numberOfOperations, bucket);
			if (!debugAsync)
			{
				var ptimea = Math.Round(((ktimes - ktimea) / ktimes) * 100);
				Console.WriteLine("Ketchup Async: " + ktimea + " seconds (" + ptimea + "% faster)");
			}

			Run();
		}

		public static double SetAndGetKetchupSync(int numberOfOperations, Bucket cli)
		{
			var start = DateTime.Now;

			for (var i = 0; i < numberOfOperations; i++)
			{
			    var key = "kc" + i;
			    var value = key + " value";
			    cli.Set(key, value);
			}

			for (var i = 0; i < numberOfOperations; i++)
			{
				var key = "kc" + i;
				var expected = key + " value";
				var actual = cli.Get<string>(key);
				if (actual != expected) Console.WriteLine("Values did not match. expected: " + expected + ". actual: " + actual);
			}

			return (DateTime.Now - start).TotalSeconds;
		}

		public class DemoAsyncState
		{
			public int Counter { get; set; }
			public string Key { get; set; }
		}

		public static double SetAndGetKetchupAsync(int numberOfOperations, Bucket cli)
		{
			var counter = numberOfOperations;
			var start = DateTime.Now;
			var sync = new object();

			//simulate synchronous operation
			TestAsync(180, (success, fail) =>
			{
				for (var i = 0; i < numberOfOperations; i++)
				{
					var key = "kc" + i;
					var value = key + " value";
					var asyncState = new DemoAsyncState { Key = key };
					cli.Set(key, value, s =>
						{
							lock (sync)
							{
								var state = (DemoAsyncState)s;
								var c = --counter;
								if (debugAsync) Console.WriteLine(c + ": " + state.Key + ": set");
								if (c == 0) success(s);
							}
						}, fail, asyncState);
				}
			});

			counter = numberOfOperations;
			TestAsync(60, (success, fail) =>
			{
				for (var i = 0; i < numberOfOperations; i++)
				{
					var key = "kc" + i;

					//get is fired on success return of set
					var asyncState = new DemoAsyncState { Key = key, Counter = counter };
					cli.Get<string>(key, 
						(val, s) =>
						{
							lock (sync)
							{
								var state = (DemoAsyncState)s;
								var c = --counter;
								if (debugAsync) Console.WriteLine(c + ": " + state.Key + ": " + val);
								if (c == 0) success(s);
							}
						},
						s1 =>
						{
							lock (sync)
							{
								var state1 = (DemoAsyncState)s1;
								var c1 = --counter;
								if (debugAsync) Console.WriteLine(c1 + ": " + state1.Key + ": miss");
								if (c1 == 0) success(s1);
							}
						},
						fail, asyncState
					);
				}
			});

			return (DateTime.Now - start).TotalSeconds;
		}

		public static void TestAsync(int seconds, Action<Action<object>, Action<Exception, object>> action)
		{
			var resetEvent = new ManualResetEvent(false);
			Exception exception = null;

			action(
				//delegate to invoke on test success
				s =>
				{
					resetEvent.Set();
				},

				//delegate to invoke on test error
				(e, s) =>
				{
					exception = e;
					resetEvent.Set();
				});

			if (!resetEvent.WaitOne(seconds * 1000))
				throw new TimeoutException("Operation timed out before receiving a WaitHandle.Set();");

			if (exception != null)
				throw exception;
		}
	}
}

