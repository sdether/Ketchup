using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;

namespace Ketchup {
	internal class NodeList : List<Node> {

		public Node GetById(string id) {
			return this.FirstOrDefault(d => d.Id == id);
		}

		public Node GetOrCreate(string endpoint) {
			var host = endpoint.Split(':')[0];

			var port = GetPort(endpoint);
			var id = host + ":" + port.ToString();

			Node node = GetById(id);
			if(node != null)
				return node;

			node = new Node(host, port);
			Add(node);
			return node;
		}

		private int GetPort(string endpoint) {
			int port;

			var portString = endpoint.Contains(":") ? endpoint.Split(':')[1] : "11211";
			if (!int.TryParse(portString, out port))
				throw new ConfigurationErrorsException("The specified port is not a valid int integer");

			return port;
		}
	}
}
