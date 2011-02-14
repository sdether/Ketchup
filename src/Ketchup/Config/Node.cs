using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Ketchup {
	
	internal class Node {

		public string Host { get; set; }
		public int Port { get; set; }
		public bool IsDead { get; set; }
		public int CurrentRetryCount { get; set; }
		public DateTime DeadAt { get; set; }
		public DateTime LastConnectionFailure { get; set; }

		public string Id {
			get { return Host + ":" + Port; }
		}

		public Node() {
			IsDead = false;
			CurrentRetryCount = 0;
			DeadAt = DateTime.MinValue;
			LastConnectionFailure = DateTime.MinValue;
		}

		public Node(string host, int port) : this() {
			Host = host;
			Port = port;
		}
	}
}
