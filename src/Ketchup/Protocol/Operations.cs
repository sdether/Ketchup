using System;
using Ketchup.Config;
using Ketchup.Hashing;

namespace Ketchup.Protocol {
	internal static class Operations {
		private static readonly KetchupConfig config = KetchupConfig.Current;

		internal static void Get<T>(Op operation, string key, string bucket, Action<T> hit, Action miss, Action<Exception> error) {
			key = config.GetKey(key, bucket);
			var packet = new Packet<T>(operation, key);
			Hasher.GetNode(key, bucket).Request(packet.Serialize(),
				rb => {
					try {
						hit(packet.Deserialize(rb));
					} catch (Exception ex) {
						if (ex is NotFoundException) {
							miss();
						} else {
							error(ex);
						}

					}
				});
		}

		internal static void Set<T>(Op operation, string key, T value, int expiration, string bucket, Action success, Action<Exception> error) {
			//may move the extras parser into the packet itself
			var extras = new byte[8];
			expiration.CopyTo(extras, 4);

			key = config.GetKey(key, bucket);
			var packet = new Packet<T>(operation, key).Extras(extras).Value(value);
			Hasher.GetNode(key, bucket).Request(packet.Serialize(),
				rb => {
					try {
						packet.Deserialize(rb);
						success();
					} catch (Exception ex) {
						error(ex);
					}
				}
			);
		}
	}
}
