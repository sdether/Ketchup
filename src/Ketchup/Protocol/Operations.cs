using System;
using Ketchup.Config;
using Ketchup.Hashing;
using Ketchup.Protocol.Exceptions;

namespace Ketchup.Protocol {
	internal static class Operations {
		private static readonly KetchupConfig config = KetchupConfig.Current;

		public static void Get<T>(Op operation, string key, string bucket, Action<T> hit, Action miss, Action<Exception> error) {
			var packet = new Packet<T>(operation, config.GetKey(key, bucket));
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

		public static void SetAddReplace<T>(Op operation, string key, T value, int expiration, string bucket, Action success, Action<Exception> error) {
			//may move the extras parser into the packet itself
			var extras = new byte[8];
			expiration.CopyTo(extras, 4);

			key = config.GetKey(key, bucket);
			var packet = new Packet<T>(operation, key).Extras(extras).Value(value);

			if(success != null && error != null) {
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
			else {
				//shh...
				Hasher.GetNode(key, bucket).Request(packet.Serialize(), null);
			}
		}

		public static void Delete(Op operation, string key, string bucket, Action success, Action<Exception> error) {
			var packet = new Packet<string>(operation, config.GetKey(key, bucket));
			Hasher.GetNode(key, bucket).Request(packet.Serialize(),
				rb => {
					try {
						packet.Deserialize(rb);
						success();
					} catch (Exception ex) {
						error(ex);
					}
				});
		}

		public static void IncrDecr(Op operation, string key, long step, long initial, int expiration, string bucket, Action<ulong> success, Action<Exception> error) {
			var extras = new byte[20];
			step.CopyTo(extras, 0);
			initial.CopyTo(extras, 8);
			expiration.CopyTo(extras, 16);
			var packet = new Packet<ulong>(operation, config.GetKey(key, bucket)).Extras(extras);
			Hasher.GetNode(key, bucket).Request(packet.Serialize(),
				rb => {
					try {
						success(packet.Deserialize(rb));
					}
					catch (Exception ex) {
						error(ex);
					}
				});
		}
	}
}
