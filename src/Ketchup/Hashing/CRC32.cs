using System;

namespace Ketchup.Hashing{
	internal class Crc32 {
		readonly uint[] table;
		public uint GetUInt(byte[] bytes) {
			var crc = 0xffffffff;
			foreach (var t in bytes)
			{
				var index = (byte)(((crc) & 0xff) ^ t);
				crc = (crc >> 8) ^ table[index];
			}
			return ~crc;
		}

		public byte[] GetBytes(byte[] bytes) {
			return BitConverter.GetBytes(GetUInt(bytes));
		}

		public Crc32() {
			const uint poly = 0xedb88320;
			table = new uint[256];
			for (uint i = 0; i < table.Length; ++i) {
				var temp = i;
				for (var j = 8; j > 0; --j) {
					if ((temp & 1) == 1) {
						temp = (temp >> 1) ^ poly;
					} else {
						temp >>= 1;
					}
				}
				table[i] = temp;
			}
		}
	}

}
