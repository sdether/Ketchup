using System.Collections.Generic;
using System.Linq;

namespace Ketchup {
	internal class NodeList : List<Node> {

		public Node GetById(string id) {
			return this.FirstOrDefault(d => d.Id == id);
		}

		public Node GetOrCreate(string endpoint) {
			var host = endpoint.Split(':')[0];

			var port = Node.GetPort(endpoint);
			var id = host + ":" + port;

			var node = GetById(id);
			if(node != null)
				return node;

			node = new Node(host, port);
			Add(node);
			return node;
		}


	}
}
