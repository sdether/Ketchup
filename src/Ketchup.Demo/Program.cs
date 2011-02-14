using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Ketchup;
using Ketchup.Config;

namespace Ketchup.Demo {
	public class Program {
		static void Main(string[] args) {
			CreateConfigSection();
			Console.Read();
		}

		public static void CreateConfigSection() {
			var section = KetchupConfigSection.Instance();
			var config = section.Create();
		}
	}
}
