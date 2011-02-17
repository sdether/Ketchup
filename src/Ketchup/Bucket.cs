using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ketchup {
	public class Bucket {
		public string Name { get; set; }
		public int Port { get; set; }
		public bool Prefix { get; set; }
		public IList<string> ConfigNodes { get; private set; }
		public IDictionary<string, string> Attributes { get; private set; }

		public Bucket() {
			Name = "default";
			Port = 0;
			Prefix = true;
			ConfigNodes = new List<string>();
			Attributes = new Dictionary<string, string>();
		}
	}
}
