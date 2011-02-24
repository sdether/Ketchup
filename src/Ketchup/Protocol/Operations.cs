using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ketchup.Config;
using Ketchup.Protocol;

namespace Ketchup.Protocol {
	internal static class Operations {
		private static KetchupConfig config = KetchupConfig.Current;
		
		internal static void Get<T>(Op operation,string key,string bucket,Action<T> hit,Action miss,Action<Exception> error) {
			key = config.GetKey(key, bucket);
			var packet = new Packet<T>().Operation(operation).Key(key);
			var node = Hashing.GetNode(key, bucket).Request(packet.Serialize(),
				rb => { hit(packet.Deserialize(rb)); },
				ex => {
					if (ex is Protocol.NotFoundException) {
						miss();
					} else {
						error(ex);
					}
				}
			);
		}

		internal static void Set<T>(Op operation, string key, T value, int expiration, string bucket, Action success, Action<Exception> error) {
			//may move the extras parser into the packet itself
			var extras = new byte[8];
			expiration.CopyTo(extras, 4);

			key = config.GetKey(key, bucket);
			var packet = new Packet<T>(operation).Key(key).Extras(extras).Value(value);
			var node = Hashing.GetNode(key, bucket).Request(packet.Serialize(),
				rb => { success(); },
				ex => { error(ex); }
			);
		}
	}
}
