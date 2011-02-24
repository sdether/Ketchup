using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Ketchup;
using Ketchup.Asynchronous;
using Ketchup.Config;
using System.Threading;

namespace Ketchup.Demo {
	public class Program {
		static void Main(string[] args) {
			//CreateConfigSection();
			GetValueWithKey();
			Console.Read();
		}

		public static void CreateConfigSection() {
			var section = KetchupConfigSection.Current;
			var config = KetchupConfig.Current;
		}

		public static void GetValueWithKey() {
			var cli = new KetchupClient("default");
			var misses = 0;

			for (var i = 0; i < 100; i++) {
				var j = i;
				Console.WriteLine("get " + i + " at " + DateTime.Now);
				cli.Get<string>(
				"Hello",
				//hit
				val => {
					Console.WriteLine("hit " + j + " at " + DateTime.Now + ". value: " + val);
				},
				//miss
				() => {
					Console.WriteLine("miss " + j + " at " + DateTime.Now);
					misses++;
					Console.WriteLine("total misses: " + misses);
				},
				//error
				ex => {
					Console.WriteLine("error " + j + " at " + DateTime.Now);
					Console.Write(ex);
				});
			}
		}
	}
}
