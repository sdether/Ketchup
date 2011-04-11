using System;
using Ketchup;
using Ketchup.Async;
using Ketchup.Config;

public class Program
{
	private static Bucket _bucket;
	private static readonly string _key = "key-async";
	private static readonly string _value = "key-async-value";

	static void Main(string[] args)
	{
		//Initialize Ketchup Client
		var config = new KetchupConfig("default", "172.17.6.201", 11211);
		var client = new KetchupClient(config);
		_bucket = client.GetBucket("default");

		//Set asyncrhonously, call OnSetSuccess() on success and OnSetError on Exception
		_bucket.Set(_key, _value, OnSetSuccess<string>, OnSetError, null);
		Console.ReadLine();
	}

	public static void OnSetError(Exception ex, object state)
	{
		Console.WriteLine("Set command for key '" + _key + "' failed with error " + ex.Message);
		Console.WriteLine("Pess any key to continue...");
	}

	public static void OnSetSuccess<T>(object state)
	{
		//set was successful, try Get
		_bucket.Get<T>(_key, OnGetHit, OnGetMiss, OnGetError, null);
	}

	public static void OnGetMiss(object state)
	{
		Console.WriteLine("Get command for key '" + _key + "' returned miss");
		Console.WriteLine("Pess any key to continue...");
	}

	public static void OnGetError(Exception ex, object state)
	{
		Console.WriteLine("Get command for key '" + _key + "' failed with exception '" + ex.Message + "'");
		Console.WriteLine("Pess any key to continue...");
	}

	public static void OnGetHit<T>(T value, object state)
	{
		//Hit was successful, now delete it
		_bucket.Delete(_key, OnDeleteSuccess, OnDeleteError, null);
	}

	public static void OnDeleteError(Exception ex, object state)
	{
		Console.WriteLine("Delete command for key '" + _key + "' failed with exception '" + ex.Message + "'");
		Console.WriteLine("Pess any key to continue...");
	}

	public static void OnDeleteSuccess(object state)
	{
		Console.WriteLine("Set, Get and Delete commands for key '" + _key + "' were successful");
		Console.WriteLine("Pess any key to continue...");
	}
}
