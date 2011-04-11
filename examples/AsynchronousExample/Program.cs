using System;
using Ketchup;
using Ketchup.Async;
using Ketchup.Config;

public class Program
{
	private static Bucket _bucket;
	private static readonly string _key = "key-async";
	private static readonly string _value = "key-async-value";

	public static void Main(string[] args)
	{
		//Initialize Ketchup Client
		var config = new KetchupConfig("default", "127.0.0.1", 11211);
		var client = new KetchupClient(config);
		_bucket = client.GetBucket("default");

		//Set asyncrhonously, call OnSetSuccess() on success and OnSetError on Exception
		_bucket.Set(_key, _value, OnSetSuccess<string>, OnSetError, null);
	}

	private static void OnSetSuccess<T>(object state)
	{
		//Set was successful, now execute Get async
		//Pass callbacks for OnGetHit, OnGetMiss, OnGetError
		_bucket.Get<T>(_key, OnGetHit, OnGetMiss, OnGetError, null);
	}

	private static void OnGetHit<T>(T value, object state)
	{
		//OnGetHit success, now execute Delete
		//Pass callbacks for OnDeleteSuccess and OnDeleteError
		_bucket.Delete(_key, OnDeleteSuccess, OnDeleteError, null);
	}

	private static void OnDeleteSuccess(object state)
	{
		//Asynchronous path through Set, Get, Delete was successul, exit program
		Console.WriteLine("Set, Get and Delete commands for key '" + _key + "' were successful");
		Finish();
	}

	private static void Finish()
	{
		Console.WriteLine("Pess any key to continue...");
		Console.ReadLine();
	}

	private static void OnGetMiss(object state)
	{
		Console.WriteLine("Get command for key '" + _key + "' returned miss");
		Finish();
	}

	private static void OnSetError(Exception ex, object state)
	{
		Console.WriteLine("Set command for key '" + _key + "' failed with error " + ex.Message);
		Finish();
	}

	private static void OnGetError(Exception ex, object state)
	{
		Console.WriteLine("Get command for key '" + _key + "' failed with exception '" + ex.Message + "'");
		Finish();
	}

	private static void OnDeleteError(Exception ex, object state)
	{
		Console.WriteLine("Delete command for key '" + _key + "' failed with exception '" + ex.Message + "'");
		Finish();
	}
}
