using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ketchup.Protocol;
using Ketchup.Config;

namespace Ketchup.Protocol.Binary {

	internal static class GetExtensions {
		private static KetchupConfig config = KetchupConfig.Current;

		public static KetchupClient Get(this KetchupClient client, string key, string bucket, Action<IList<ArraySegment<byte>>> callback) {
			key = config.GetKey(key,bucket);
			var packet = new Packet()
				.Magic(Magic.Request)
				.Op(Op.Get)
				.Key(key)
				.Create();

			var node = client
				.Node(key, bucket)
				.Request(packet,callback);

			return client;
		}

	}
}

