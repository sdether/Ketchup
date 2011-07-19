using System;
using Ketchup;
using Ketchup.Config;
using Ketchup.Async;

namespace MonoSpymemcachedLikeExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var config = new KetchupConfig()
				.AddBucket("default",11211)
				.AddNode("server1")
				.AddNode("server2");
					
			var bucket = new KetchupClient(config).DefaultBucket;
			var myObj = default(object);
			bucket.Get<object>("somekey", 
			    (v,s) => {
					//hit
					myObj = v;
				},
				(s) => {
					//miss, add miss logic here
				},
				(e, s) => {
					//exception, log error or fail operation
				},
				null
			);
			
			Console.WriteLine(myObj);
		}
	}
}

