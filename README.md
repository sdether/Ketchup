**Ketchup Public API**

Since Ketchup is multi-tenant out of the box, all commands are executed against a Ketchup "Bucket".

Buckets are specified via configuration and you access a Bucket via the GetBucket(string name) method:

	var client = new KetchupClient();
	var bucket = client.GetBucket("default")l

If you do not need multi-tenancy, you only have one bucket you can shortcut:
	
	var bucket = new KetchupClient().Default;

Once you have selected a Bucket, you can execute Memcached API commands. 

Ketchup supports 4 versions of the Memcached API commands:

1. Synchronous - returns on main thread
2. Asynchronous - executes callbacks on second thread [View Example](https://github.com/jasonsirota/Ketchup/blob/master/examples/AsynchronousExample/Program.cs)
3. Quiet - executes callbacks on second thread, suppresses uninteresting responses
4. Silent - executes callbacks on second thread, suppresses uninteresting responses *and* exceptions

By default, Ketchup attempts to choose the most performant options:

* Sets and Deletes are executed silently
* Gets are executed synchronously

Example:

		using Ketchup;

		public class Program
		{
			static void Main(string[] args)
			{
				var key = "key";
				var value = "key-value";
				var bucket = new KetchupClient().Default;

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

However, you can expose Asynchronous, Synchronous, Quiet or Silent commands by using Extension methods specified by a using statement

* Synchronous: using Ketchup.Sync
* Asynchronous: using Ketchup.Async [View Example](https://github.com/jasonsirota/Ketchup/blob/master/examples/AsynchronousExample/Program.cs)
* Quiet: using Ketchup.Quiet
* Silent: using Ketchup.Silent

Synchronous Example:

		using Ketchup.Sync

		public class Program
		{
			static void Main(string[] args)
			{
				var key = "key-sync";
				var value = "key-sync-value";
				var bucket = new KetchupClient().Default;

				//Set
				if (!bucket.Set(key, value))
					Console.WriteLine("Setting key " + key + " failed.");

				var expected = value;
				var actual = bucket.Get<string>(key);

				Console.WriteLine("Expected: " + expected + " Actual: " + actual + " Match: " + (expected == actual).ToString());

				if(!bucket.Delete(key))
					Console.WriteLine("Deleting key " + key " failed.");
			}
		}

Quiet Example - like Async only uninteresting responses are suppressed

		using Ketchup.Async

		public class Program
		{
			static void Main(string[] args)
			{
				var key = "key-quiet";
				var value = "key-quiet-value";
				var state = default(object);
				var bucket = new KetchupClient().Default;

				//Quiet Set only returns on exception
				bucket.Set(key, value, 
					(exception, stateException) => 
					{
						Console.WriteLine("Set command for key " + key + " failed"); 
					},
					state
				);

				//Quiet Get only returns on hit or exception
				bucket.Get(key,
					(val, stateGetHit) =>
					{
						Console.WriteLine("Get command for key " + key + " returned value " + value);
					},
					(exceptionGet,stateGetException) =>
					{
						Consoles.WriteLine("Get command for key " + key + " failed");
					},
					state
				);

				//Quiet Delete only returns on exception
				bucket.Delete(key,
					(exceptionDelete) =>
					{
						Console.WriteLine("Delete command for key " + key + " failed.");
					},
					state
				);
			}
		}

Silent Example - like Quiet only Exceptions are suppressed

		using Ketchup.Async

		public class Program
		{
			static void Main(string[] args)
			{
				var key = "key-quiet";
				var value = "key-quiet-value";
				var state = default(object);
				var bucket = new KetchupClient().Default;

				//Silent set never returns
				bucket.Set(key, value);

				//Silent Get only returns on hit
				bucket.Get(key,
					(val, stateGetHit) =>
					{
						Console.WriteLine("Get command for key " + key + " returned value " + value);
					},
					state
				);

				//Silent delete never returns
				bucket.Delete(key);
			}
		}