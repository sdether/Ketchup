using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ketchup.Config;

namespace Ketchup.Protocol.Asynchronous {
	internal static class Operations {
		private static KetchupConfig config = KetchupConfig.Current;
		internal static void Get<T>(
			Op operation,
			string key,
			string bucket,
			Action<T> hit,
			Action miss,
			Action<Exception> error
			) {
			try {
				key = config.GetKey(key, bucket);
				var packet = new Packet<T>().Operation(operation).Key(key);

				var node =  NodeExtensions.GetNode(key, bucket).Request(packet.Serialize(),
					rb => { hit(packet.Deserialize(rb)); },
					ex => {
						if (ex is Protocol.NotFoundException) {
							miss();
						} else {
							error(ex);
						}
					}
				);
			} catch (Exception ex) {
				error(ex);
			}
		}
	}
}
