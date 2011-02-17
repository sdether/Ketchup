using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ketchup.Config;
using System.Security.Cryptography;
using Ketchup.Algorithms;

namespace Ketchup.Protocol {

	internal static class HashExtensions {
		private static CRC32 crc32 = new CRC32();

		public static Node Node(this KetchupClient client, string key, IList<Node> nodes){
			int hash = 0;
			switch (KetchupConfig.Current.HashingAlgorithm) {
				case HashingAlgortihm.Ketama:
					hash = KetamaHash(key);
					break;
				default:
					hash = DefaultHash(key);
					break;
			}

			var idx = hash % nodes.Count;
			return nodes[idx];
		}

		private static int DefaultHash(string key) {
			//we are not stealing government secrets here, first 32 bits should be sufficient for an even distribution
			var hash = crc32.GetBytes(Encoding.UTF8.GetBytes(key));
			return BitConverter.ToInt32(hash, 0);
		}

		private static int KetamaHash(string key) {
			throw new NotImplementedException();
		}



	}
}
