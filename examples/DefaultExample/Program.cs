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
