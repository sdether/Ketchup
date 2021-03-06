﻿using System;
using System.Text;
using Ketchup.Config;

namespace Ketchup.Hashing {

	public static class Hasher {
		private static readonly KetchupConfig config = KetchupConfig.Current;
		private static readonly Crc32 crc32 = new Crc32();

		public static Node GetNode(Bucket bucket, string key) {
			return GetNode(bucket, key, config.HashingAlgorithm);
		}

		public static Node GetNode(Bucket bucket, string key, HashingAlgortihm hashAlgorithm)
		{
			int hash;
			switch (hashAlgorithm) {
				case HashingAlgortihm.Ketama:
					hash = KetamaHash(key);
					break;
				default:
					hash = DefaultHash(key);
					break;
			}

			var nodes = config.GetNodes(bucket);
			var idx = Math.Abs(hash % nodes.Count);
			return nodes[idx];
		}

		public static Node Node(this Bucket bucket, string key) {
			return GetNode(bucket, key);
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
