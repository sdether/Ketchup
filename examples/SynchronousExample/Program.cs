﻿using System;
using Ketchup;
using Ketchup.Config;
using Ketchup.Sync;

namespace SynchronousExample
{
	public class Program
	{
		private static readonly Bucket _bucket = new KetchupClient("127.0.0.1", 11211).DefaultBucket;
		private static readonly string _key = "key-sync";
		private static readonly string _value = "key-sync-value";

		public static void Main(string[] args)
		{
			//Set
			if (!_bucket.Set(_key, _value)) 
				Console.WriteLine("Setting key " + _key + " failed.");

			var expected = _value;

			//Get
			var actual = _bucket.Get<string>(_key);

			Console.WriteLine("Expected: " + expected + " Actual: " + actual + " Match: " + (expected == actual).ToString());

			//Delete
			if(!_bucket.Delete(_key)) 
				Console.WriteLine("Deleting key " + _key + " failed.");

			Console.WriteLine("Set, Get and Delete commands for key '" + _key + "' were successful");
			Finish();
		}

		private static void Finish()
		{
			Console.WriteLine("Pess any key to continue...");
			Console.ReadLine();
		}

	}
}
