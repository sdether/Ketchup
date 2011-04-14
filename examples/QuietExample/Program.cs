using System;
using Ketchup;
using Ketchup.Config;
using Ketchup.Quiet;

namespace QuietExample
{
    public class Program
    {
        static void Main(string[] args)
        {
			var bucket = new KetchupClient("localhost", 11211).DefaultBucket;
			var key = "key-quiet";
			var value = "key-quiet-value";
			var state = default(object);

			//Quiet Set only returns on exception
            bucket.Set(key, value, 
                (exception, stateException) => 
                {
                    Console.WriteLine("Set command for key " + key + " failed"); 
                },
                null
            );

            //Quiet Get only returns on hit or exception
            bucket.Get<string>(key,
                (val, stateGetHit) =>
                {
                    Console.WriteLine("Get command for key " + key + " returned value " + value);
                },
                (exceptionGet,stateGetException) =>
                {
                    Console.WriteLine("Get command for key " + key + " failed");
                },
                state
            );

            //Quiet Delete only returns on exception
            bucket.Delete(key,
                (exceptionDelete, stateDelete) =>
                {
                    Console.WriteLine("Delete command for key " + key + " failed.");
                	Finish();
                },
                state
            );

        	Console.ReadLine();
        }

		private static void Finish()
		{
			Console.WriteLine("Pess any key to continue...");
			Console.ReadLine();
		}

    }
}
