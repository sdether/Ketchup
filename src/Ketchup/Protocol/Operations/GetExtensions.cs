using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ketchup.Protocol;

namespace Ketchup.Protocol.Binary {

	internal static class GetExtensions {

		public static KetchupClient Get(this KetchupClient client, string key, IList<Node> nodes, Action<ArraySegment<byte>, ushort> callback) {
			var node = client.Node(key, nodes).Connect();
			return client;
		}

	}
}
