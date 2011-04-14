using System;
using Ketchup;
using Ketchup.Silent;

    public class Program
    {
        public static void Main(string[] args)
        {
			var bucket = new KetchupClient("localhost", 11211).DefaultBucket;
			var key = "key-silent";
			var value = "key-silent-value";
			var state = default(object);

            //Silent set never returns
            bucket.Set(key, value);

            //Silent Get only returns on hit
            bucket.Get<string>(key,
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