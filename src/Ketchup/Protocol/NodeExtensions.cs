using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ketchup.Config;
using System.Security.Cryptography;
using Ketchup.Algorithms;

namespace Ketchup.Protocol {

	internal static class NodeExtensions {
		private static CRC32 crc32 = new CRC32();
		private static KetchupConfig config = KetchupConfig.Current;

		internal static Node GetNode(string key, string bucket) {
			int hash = 0;
			switch (KetchupConfig.Current.HashingAlgorithm) {
				case HashingAlgortihm.Ketama:
					hash = KetamaHash(key);
					break;
				default:
					hash = DefaultHash(key);
					break;
			}

			var nodes = config.GetNodes(bucket);
			var idx = hash % nodes.Count;
			return nodes[idx];
		}

		public static Node Node(this KetchupClient client, string key, string bucket) {
			return GetNode(key, bucket);
		}

		private static int DefaultHash(string key) {
			//we are not stealing government secrets here, first 32 bits should be sufficient for an even distribution
			//TODO: determine if you do or do not need a 
			var hash = crc32.GetBytes(Encoding.UTF8.GetBytes(key));
			return BitConverter.ToInt32(hash, 0);
		}

		private static int KetamaHash(string key) {
			throw new NotImplementedException();
		}



	}
}
