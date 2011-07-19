using System;
using Ketchup;
using Ketchup.Async;
using Ketchup.Config;

namespace MonoAsynchronousExample
{
	class MainClass
	{
		//Initialize Ketchup Client
		private static Bucket _bucket;
		private static readonly string _key = "key-async-mono-4";
		private static readonly string _value = "key-async-value";
	
		public static void Main(string[] args)
		{
			//create a config class but set Event Loop for Mono
			var config = new KetchupConfig() {
				UseEventLoop = false
			}.AddNode("127.0.0.1:11211").AddBucket();
			
			//Instantiating the client starts the eventloop
			_bucket = new KetchupClient(config).DefaultBucket;
			
			//Set asyncrhonously, call OnSetSuccess() on success and OnSetError on Exception
			Console.WriteLine("Begin Set");
			var state = default(object);
			_bucket.Set(_key, _value, OnSetSuccess<string>, OnSetError, state);
			
			//keep the main thread active until the worker threads complete
			Console.ReadLine();
		}
	
		private static void OnSetSuccess<T>(object state)
		{
			//Set was successful, now execute Get async
			//Pass callbacks for OnGetHit, OnGetMiss, OnGetError
			_bucket.Get<T>(_key, OnGetHit, OnGetMiss, OnGetError, state);
		}
	
		private static void OnGetHit<T>(T value, object state)
		{
			//OnGetHit success, now execute Delete
			//Pass callbacks for OnDeleteSuccess and OnDeleteError
			_bucket.Delete(_key, OnDeleteSuccess, OnDeleteError, state);
		}
	
		private static void OnDeleteSuccess(object state)
		{
			//Asynchronous path through Set, Get, Delete was successul, exit program
			Console.WriteLine("Set, Get and Delete commands for key '" + _key + "' were successful");
		}

		private static void OnGetMiss(object state)
		{
			Console.WriteLine("Get command for key '" + _key + "' returned miss");
		}
	
		private static void OnSetError(Exception ex, object state)
		{
			Console.WriteLine("Set command for key '" + _key + "' failed with error " + ex.Message);
		}
	
		private static void OnGetError(Exception ex, object state)
		{
			Console.WriteLine("Get command for key '" + _key + "' failed with exception '" + ex.Message + "'");
		}
	
		private static void OnDeleteError(Exception ex, object state)
		{
			Console.WriteLine("Delete command for key '" + _key + "' failed with exception '" + ex.Message + "'");
		}
	}
}

