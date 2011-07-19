using System;
using System.Diagnostics;
using Ketchup;
using Ketchup.Async;
using Ketchup.Sync;
using System.Threading;
using System.Collections.Generic;

namespace MonospaceAsyncSetExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var bucket = new KetchupClient("localhost",11211).DefaultBucket;
			var opcount = 100;
			var sw = new Stopwatch();
			bucket.Flush();
			
			//sync
			var syncBaseKey = "sync-set";
			sw.Start();
			for(var i = 0; i<opcount; i++)
			{
				var key = syncBaseKey + "-" + i;
				string cacheValue = null;
				cacheValue = bucket.Get<string>(key);
				Console.WriteLine(key + ": get");
				if(cacheValue == null)
				{
					var val = key + "-value";
					var success = bucket.Set(key,val);
					Console.WriteLine(key + ": set " + success);
				}
			}
			sw.Stop();
			var syncTime = (double)sw.ElapsedMilliseconds/1000;
			
			//async set
			sw.Reset();
			var asyncSetBaseKey = "async-set";
			sw.Start();
			for(var i = 0; i<opcount; i++)
			{
				var key = asyncSetBaseKey + "-" + i;
				var cacheValue = bucket.Get<string>(key);
				Console.WriteLine(key + ": get");
				if(cacheValue == null)
				{
					var val = key + "-value";
					bucket.Set(key,val,null,null,null);
					Console.WriteLine(key + ": set");
				}
			}
			sw.Stop();
			var asyncTime = (double)sw.ElapsedMilliseconds/1000;
			
			//async get and set
			sw.Reset();
			var asyncGetSetBaseKey = "async-get-set";
			var keys = new List<string>();
			var mre = new ManualResetEvent(false);
			sw.Start();
			for(var i = 0; i<opcount; i++)
			{
				var key = asyncGetSetBaseKey + "-" + i;
				keys.Add(key);
				Console.WriteLine(key + ": get");
				bucket.Get<string>(key,
				    //hit
					(v,s) => {
						var hitkey = (string)s;
						Console.WriteLine(hitkey + ": hit");
						keys.Remove(hitkey);
						if(keys.Count == 0) mre.Set();
					},
					//miss
					(s) => {
						var misskey = (string)s;
						Console.WriteLine(misskey + ": miss");
					
						//set the value
						var val = misskey + "-value";
						bucket.Set(misskey, val, null,null,null);
						Console.WriteLine(misskey + ": set");
						keys.Remove(misskey);
						if(keys.Count == 0) mre.Set();
					},
					//error
					(e,s) => {
						var errorkey = (string)s;
						Console.WriteLine(errorkey + ": error");
						Console.WriteLine(e.Message);
						keys.Remove(errorkey);
						if(keys.Count == 0) mre.Set();
					},
					key
				);
				
			}
			mre.WaitOne();
			var asyncGetTime = (double)sw.ElapsedMilliseconds/1000;
			
			Console.WriteLine(opcount + " Operations, Sync: " + syncTime + "s");
			Console.WriteLine(opcount + " Operations, Async Set: " + asyncTime + "s");                
			Console.WriteLine(opcount + " Operations, Async Get Set: " + asyncGetTime + "s");                
		}
	}
}