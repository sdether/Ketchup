using System;
using System.Threading;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Ketchup.Async;
using Ketchup.Config;

namespace Ketchup.Demo {
	public class Program {
		static void Main(string[] args) {
			ReadLine();
		}

		private static void ReadLine() {
			Console.WriteLine("Enter the number of operations or enter to quit");
			var numberOfOperations = 0;
			if (int.TryParse(Console.ReadLine(), out numberOfOperations)) {

				//var ecli = new MemcachedClient();
				var kcli = new KetchupClient("default");
				//Console.WriteLine("Number of Operations: " + numberOfOperations);
				
				//var etime = SetAndGetEnyim(numberOfOperations, ecli);
				//Console.WriteLine("Enyim: " + etime + " milliseconds");
				
				//var ktimes = SetAndGetKetchupSync(numberOfOperations, kcli);
				//var ptimes = Math.Round(((etime - ktimes) / etime) * 100);
				//Console.WriteLine("Ketchup Sync: " + ktimes + " milliseconds (" + ptimes + "% faster)");
				
				var ktimea = SetAndGetKetchupAsync(numberOfOperations, kcli);
				//var ptimea = Math.Round(((etime - ktimea) / etime) * 100);
				//Console.WriteLine("Ketchup Async: " + ktimea + " milliseconds (" + ptimea + "% faster)");
				
				ReadLine();
			}
		}

		public static double SetAndGetEnyim(int numberOfOperations, IMemcachedClient cli) {
			var start = DateTime.Now;

			//enyim is synchronous so no need to handle callbacks
			for (int i = 0; i < numberOfOperations; i++) {
				var key = "ey" + i;
				var value = key + " value";
				if (cli.Store(StoreMode.Set, key, value)) {
					//Console.WriteLine(key + ": set ");
				} else {
					throw new Exception("Enyim fail on set");
				}
			}

			for (int i = 0; i < numberOfOperations; i++) {
				var key = "ey" + i;
				var value = cli.Get(key);
				//Console.WriteLine(key + ": " + (value == null ? "miss" : value));
			}

			return (DateTime.Now - start).TotalMilliseconds;
		}

		public static double SetAndGetKetchupSync(int numberOfOperations, KetchupClient cli) {
			var start = DateTime.Now;

			for (int i = 0; i < numberOfOperations; i++) {
				var key = "kc" + i;
				var value = key + " value";

				TestAsync(5, (success, fail) => {
					cli.Set(key, value, () => {
						//Console.WriteLine(key + ": set");
						success();
					}, (ex) => { 
						fail(ex);
					});
				});
			}

			for (int i = 0; i < numberOfOperations; i++) {
				var key = "kc" + i;
				TestAsync(5, (success, fail) => {
					cli.Get<string>(key, (val) => {
						//Console.WriteLine(key + ": " + val);
						success();
					},
					() => {
						//Console.WriteLine(key + ": miss");
						success();
					},
					fail);
				});
			}

			return (DateTime.Now - start).TotalMilliseconds;
		}

		public static double SetAndGetKetchupAsync(int numberOfOperations, KetchupClient cli) {
			var counter = 0;
			var start = DateTime.Now;

			//simulate synchronous operation
			TestAsync(60, (success, fail) => {
				for (int i = 0; i < numberOfOperations; i++) {
					var key = "kc" + i;
					var value = key + " value";
					cli.Set(key, value, () => {
						counter++;
						Console.WriteLine(counter + ": set");
						if (counter >= numberOfOperations)
							success();
					}, (ex) => { 
						fail(ex);
					});
					//end set
				}
			});

			counter = 0;
			TestAsync(60, (success, fail) => {
				for (int i = 0; i < numberOfOperations; i++) {
					var key = "kc" + i;
					//get is fired on success return of set
					cli.Get<string>(key, (val) => {
						counter++;
						//Console.WriteLine(counter + ": " + val);
						if (counter >= numberOfOperations)
							success();
					},
					() => {
						counter++;
						//Console.WriteLine(counter + ": miss");
						if (counter >= numberOfOperations)
							success();
					},
					fail);
				}
			});

			return (DateTime.Now - start).TotalMilliseconds;
		}

		public static void TestAsync(int seconds, Action<Action, Action<Exception>> action) {
			var resetEvent = new ManualResetEvent(initialState: false);
			Exception exception = null;

			action(
				//delegate to invoke on test success
				() => {
					resetEvent.Set();
				},

				//delegate to invoke on test error
				(e) => {
					exception = e;
					resetEvent.Set();
				});

			if (!resetEvent.WaitOne(seconds * 1000))
				throw new TimeoutException("Operation timed out before receiving a WaitHandle.Set();");

			if (exception != null)
				throw exception;

		}

		public static void CreateConfigSection() {
			var section = KetchupConfigSection.Current;
			var config = KetchupConfig.Current;
		}


		public static void GetValueWithKey() {
			var cli = new KetchupClient("default");
			for (var i = 0; i < 100; i++) {
				var misses = 0;
				var j = i;
				Console.WriteLine("get " + i + " at " + DateTime.Now);
				cli.Get<string>(
				"Hello",

				(val) => Console.WriteLine("hit " + j + " at " + DateTime.Now + ". value: " + val),
					//miss
				() => {
					Console.WriteLine("miss " + j + " at " + DateTime.Now);
					misses++;
					Console.WriteLine("total misses: " + misses);
				},
					//error
				(ex) => {
					Console.WriteLine("error " + j + " at " + DateTime.Now);
					Console.Write(ex);
				});
			}
		}
	}
}
