using System;
using System.Diagnostics;
using Ketchup;
using Ketchup.Async;
using Ketchup.Sync;

namespace MonospaceAsyncSetExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var bucket = new KetchupClient("localhost",11211).DefaultBucket;
			var opcount = 10;
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
				Console.WriteLine("Get " + key);
				if(cacheValue == null)
				{
					var val = key + "-value";
					var success = bucket.Set(key,val);
					Console.WriteLine("Set " + key + ": " + success);
				}
			}
			sw.Stop();
			var syncTime = (double)sw.ElapsedMilliseconds/1000;
			
			//async
			sw.Reset();
			var asyncBaseKey = "async-set";
			sw.Start();
			for(var i = 0; i<opcount; i++)
			{
				var key = asyncBaseKey + "-" + i;
				var cacheValue = bucket.Get<string>(key);
				Console.WriteLine(key + " Get");
				if(cacheValue == null)
				{
					var val = key + "-value";
					bucket.Set(key,val,null,null,null);
					Console.WriteLine(key + " Set");
				}
			}
			sw.Stop();
			var asyncTime = (double)sw.ElapsedMilliseconds/1000;
			Console.WriteLine(opcount + " Operations, Sync: " + syncTime + "s");
			Console.WriteLine(opcount + " Operations, Async: " + asyncTime + "s");                
		}
	}
}