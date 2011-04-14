**Ketchup Public API**

Since Ketchup is multi-tenant out of the box, all commands are executed against a Ketchup "Bucket".

Buckets are specified via configuration and you access a Bucket via the GetBucket(string name) method:

	var client = new KetchupClient();
	var bucket = client.GetBucket("default");

If you do not need multi-tenancy, you only have one bucket you can shortcut:
	
	var bucket = new KetchupClient().DefaultBucket;

*Or*, if you only have 1 node, and 1 bucket (not recommended) you can use:
	
	var bucket = new KetchupClient("127.0.0.1", 11211).DefaultBucket;

Once you have selected a Bucket, you can execute Memcached API commands. 

Ketchup supports 4 versions of the Memcached API commands:

1. Synchronous ([Example](https://github.com/jasonsirota/Ketchup/blob/master/examples/SynchronousExample/Program.cs)) - returns on main thread
2. Asynchronous ([Example](https://github.com/jasonsirota/Ketchup/blob/master/examples/AsynchronousExample/Program.cs)) - executes callbacks on second thread 
3. Quiet ([Example](https://github.com/jasonsirota/Ketchup/blob/master/examples/QuietExample/Program.cs)) - executes callbacks on second thread, suppresses uninteresting responses
4. Silent ([Example](https://github.com/jasonsirota/Ketchup/blob/master/examples/SilentExample/Program.cs))  - executes callbacks on second thread, suppresses uninteresting responses *and* exceptions

Ketchup attempts to make some decisions for you to implement the fastest version of the API

* Sets and Deletes are executed silently
* Gets are executed synchronously

You can expose the default decisions by with the following statement:

	using Ketchup.Commands;

Example:
		using System;
		using Ketchup;
		using Ketchup.Commands;

		namespace DefaultExample
		{
			public class Program
			{
				public static void Main(string[] args)
				{
					var bucket = new KetchupClient("localhost", 11211).DefaultBucket;
					var key = "key-default";
					var value = "key-default-value";

					//Set
					bucket.Set(key, value);

					//Get
					var expected = value;
					var actual = bucket.Get<string>(key);

					//Delete
					bucket.Delete(key);

					Console.WriteLine("Expected: " + expected + " Actual: " + actual + " Match: " + (expected == actual).ToString());
				}
			}
		}

However, you can expose Asynchronous, Synchronous, Quiet or Silent commands by using Extension methods specified by a using statement

* Synchronous ([Example](https://github.com/jasonsirota/Ketchup/blob/master/examples/SynchronousExample/Program.cs)): using Ketchup.Sync
* Asynchronous ([Example](https://github.com/jasonsirota/Ketchup/blob/master/examples/AsynchronousExample/Program.cs)): using Ketchup.Async
* Quiet ([Example](https://github.com/jasonsirota/Ketchup/blob/master/examples/QuietExample/Program.cs)): using Ketchup.Quiet
* Silent ([Example](https://github.com/jasonsirota/Ketchup/blob/master/examples/SilentExample/Program.cs)): using Ketchup.Silent