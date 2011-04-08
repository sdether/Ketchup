using System;
using System.Dynamic;
using System.Threading;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Ketchup.Async;
using Ketchup.Config;

namespace Ketchup.Demo
{
	public class Program
	{
		private static bool debugAsync = false;

		static void Main(string[] args)
		{
			ReadLine();
		}

		private static void ReadLine()
		{
			Console.WriteLine("Enter the number of operations or enter to quit");
			var numberOfOperations = 0;
			if (!int.TryParse(Console.ReadLine(), out numberOfOperations)) return;
			var ecli = new MemcachedClient();
			var kcli = new KetchupClient();
			var etime = 0d;
			Console.WriteLine("Number of Operations: " + numberOfOperations);
			if (!debugAsync)
			{
				etime = SetAndGetEnyim(numberOfOperations, ecli);
				Console.WriteLine("Enyim: " + etime + " seconds");
				var ktimes = SetAndGetKetchupSync(numberOfOperations, kcli);
				var ptimes = Math.Round(((etime - ktimes) / etime) * 100);
				Console.WriteLine("Ketchup Sync: " + ktimes + " seconds (" + ptimes + "% faster)");
			}

			var ktimea = SetAndGetKetchupAsync(numberOfOperations, kcli);
			if (!debugAsync)
			{
				var ptimea = Math.Round(((etime - ktimea) / etime) * 100);
				Console.WriteLine("Ketchup Async: " + ktimea + " seconds (" + ptimea + "% faster)");
			}

			ReadLine();
		}

		public static double SetAndGetEnyim(int numberOfOperations, IMemcachedClient cli)
		{
			var start = DateTime.Now;

			//enyim is synchronous so no need to handle callbacks
			for (var i = 0; i < numberOfOperations; i++)
			{
				var key = "ey" + i;
				var value = key + " value";
				if (cli.Store(StoreMode.Set, key, value))
				{
					//Console.WriteLine(key + ": set ");
				}
				else
				{
					throw new Exception("Enyim fail on set");
				}
			}

			for (var i = 0; i < numberOfOperations; i++)
			{
				var key = "ey" + i;
				var value = cli.Get(key);
				//Console.WriteLine(key + ": " + (value == null ? "miss" : value));
			}

			return (DateTime.Now - start).TotalSeconds;
		}

		public static double SetAndGetKetchupSync(int numberOfOperations, KetchupClient cli)
		{
			var start = DateTime.Now;

			for (var i = 0; i < numberOfOperations; i++)
			{
				var key = "kc" + i;
				var value = key + " value";
				var bucket = "default";

				TestAsync(5, (success, fail) =>
				{
					cli.Set(key, bucket, value, success, fail, new { });
				});
			}

			for (var i = 0; i < numberOfOperations; i++)
			{
				var key = "kc" + i;
				var bucket = "default";
				TestAsync(5, (success, fail) =>
				{
					cli.Get<string>(key, bucket, (val, istate) =>
					{
						//Console.WriteLine(key + ": " + val);
						success(istate);
					}, success, fail, new { });
				});
			}

			return (DateTime.Now - start).TotalSeconds;
		}

		public class DemoAsyncState
		{
			public int Counter { get; set; }
			public string Key { get; set; }
		}

		public static double SetAndGetKetchupAsync(int numberOfOperations, KetchupClient cli)
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
					var bucket = "default";
					object asyncState = new DemoAsyncState { Key = key };
					cli.Set(key, bucket, value,
						s =>
						{
							lock (sync)
							{
								dynamic state = s;
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
					var bucket = "default";

					//get is fired on success return of set
					var asyncState = new DemoAsyncState { Key = key, Counter = counter };
					cli.Get<string>(key, bucket, (val, s) =>
					{
						lock (sync)
						{
							dynamic state = s;
							var c = --counter;
							if (debugAsync) Console.WriteLine(c + ": " + state.Key + ": " + val);
							if (c == 0) success(s);
						}
					},
					s1 =>
					{
						lock (sync)
						{
							dynamic state1 = s1;
							var c1 = --counter;
							if (debugAsync) Console.WriteLine(c1 + ": " + state1.Key + ": miss");
							if (c1 == 0) success(s1);
						}
					},
					fail, asyncState);
				}
			});

			return (DateTime.Now - start).TotalSeconds;
		}

		public static void TestAsync(int seconds, Action<Action<object>, Action<Exception, object>> action)
		{
			var resetEvent = new ManualResetEvent(initialState: false);
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

		//public static void CreateConfigSection()
		//{
		//    var section = KetchupConfigSection.Current;
		//    var config = KetchupConfig.Current;
		//}
		//public static void GetValueWithKey()
		//{
		//    var cli = new KetchupClient("default");
		//    for (var i = 0; i < 100; i++)
		//    {
		//        var misses = 0;
		//        var j = i;
		//        Console.WriteLine("get " + i + " at " + DateTime.Now);
		//        cli.Get<string>(
		//        "Hello",

		//        (val) => Console.WriteLine("hit " + j + " at " + DateTime.Now + ". value: " + val),
		//            //miss
		//        () =>
		//        {
		//            Console.WriteLine("miss " + j + " at " + DateTime.Now);
		//            misses++;
		//            Console.WriteLine("total misses: " + misses);
		//        },
		//            //error
		//        (ex) =>
		//        {
		//            Console.WriteLine("error " + j + " at " + DateTime.Now);
		//            Console.Write(ex);
		//        });
		//    }
		//}
	}
}
