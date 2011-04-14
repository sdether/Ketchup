using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Ketchup {
	public static class Extensions {

		public static byte[] CopyTo(this short value, byte[] destination, int index, int length = 2) {
			var source = BitConverter.GetBytes(value);
			CopyTo(source, length, destination, index);
			return destination;
		}

		public static byte[] CopyTo(this int value, byte[] destination, int index, int length = 4) {
			var source = BitConverter.GetBytes(value);
			CopyTo(source, length, destination, index);
			return destination;
		}

		public static byte[] CopyTo(this long value, byte[] destination, int index, int length = 8) {
			var source = BitConverter.GetBytes(value);
			CopyTo(source, length, destination, index);
			return destination;
		}

		private static void CopyTo(byte[] source, int length, byte[] destination, int index) {
			var a = new byte[length];
			Array.Copy(source, 0, a, 0, length);
			Array.Reverse(a);
			Array.Copy(a, 0, destination, index, length);
		}

		public static short GetInt16(this byte[] bytes, int index, int length = 2) {
			var a = new byte[length];
			Array.Copy(bytes, index, a, 0, length);
			Array.Reverse(a);
			return BitConverter.ToInt16(a, 0);
		}

		public static int GetInt32(this byte[] bytes, int index, int length = 4) {
			var a = new byte[length];
			Array.Copy(bytes, index, a, 0, length);
			Array.Reverse(a);
			return BitConverter.ToInt32(a, 0);
		}

		public static long GetInt64(this byte[] bytes, int index, int length = 8) {
			var a = new byte[length];
			Array.Copy(bytes, index, a, 0, length);
			Array.Reverse(a);
			return BitConverter.ToInt64(a, 0);
		}

		public static byte[] Reverse(this byte[] bytes)
		{
			Array.Reverse(bytes);
			return bytes;
		}

		public static byte[] GetBytes<T>(this T obj) {
			if (obj == null) 
				return new byte[0];

			var value = (object)obj;
			switch (Type.GetTypeCode(typeof(T))) {
				case TypeCode.DBNull:
					return new byte[0];

				case TypeCode.String:
					return Encoding.UTF8.GetBytes((string)value);

				case TypeCode.Boolean:
					return BitConverter.GetBytes((bool)value).Reverse();

				case TypeCode.Int16:
					return BitConverter.GetBytes((short)value).Reverse(); 

				case TypeCode.Int32:
					return BitConverter.GetBytes((int)value).Reverse();

				case TypeCode.Int64:
					return BitConverter.GetBytes((long)value).Reverse();

				case TypeCode.UInt16:
					return BitConverter.GetBytes((ushort)value).Reverse();

				case TypeCode.UInt32:
					return BitConverter.GetBytes((uint)value).Reverse();

				case TypeCode.UInt64:
					return BitConverter.GetBytes((ulong)value).Reverse();

				case TypeCode.Char:
					return BitConverter.GetBytes((char)value);

				case TypeCode.DateTime:
					return BitConverter.GetBytes(((DateTime)value).ToBinary());

				case TypeCode.Double:
					return BitConverter.GetBytes((double)value).Reverse();

				case TypeCode.Single:
					return BitConverter.GetBytes((float)value).Reverse();

				default:
					using (var ms = new MemoryStream()) {
						new BinaryFormatter().Serialize(ms, value);
						return ms.ToArray();
					}
			}
		}

		public static T GetObject<T>(this byte[] bytes) {
			if (bytes.Length == 0) return default(T);

			object obj;

			switch (Type.GetTypeCode(typeof(T))) {
				case TypeCode.DBNull:
					obj = DBNull.Value;
					break;

				case TypeCode.String:
					obj = Encoding.UTF8.GetString(bytes);
					break;

				case TypeCode.Boolean:
					obj = BitConverter.ToBoolean(bytes, 0);
					break;

				case TypeCode.Int16:
					obj = BitConverter.ToInt16(bytes.Reverse(), 0);
					break;

				case TypeCode.Int32:
					obj = BitConverter.ToInt32(bytes.Reverse(), 0);
					break;

				case TypeCode.Int64:
					obj = BitConverter.ToInt64(bytes.Reverse(), 0);
					break;

				case TypeCode.UInt16:
					obj = BitConverter.ToUInt16(bytes.Reverse(), 0);
					break;

				case TypeCode.UInt32:
					obj = BitConverter.ToUInt32(bytes.Reverse(), 0);
					break;

				case TypeCode.UInt64:
					obj = BitConverter.ToUInt64(bytes.Reverse(), 0);
					break;

				case TypeCode.Char:
					obj = BitConverter.ToChar(bytes.Reverse(), 0);
					break;

				case TypeCode.DateTime:
					obj = new DateTime(BitConverter.ToInt64(bytes, 0));
					break;

				case TypeCode.Double:
					obj = BitConverter.ToDouble(bytes.Reverse(), 0);
					break;

				case TypeCode.Single:
					obj = BitConverter.ToSingle(bytes.Reverse(), 0);
					break;

				default:
					using (var ms = new MemoryStream(bytes)) {
						obj = new BinaryFormatter().Deserialize(ms);
					}
					break;
			}

			return (T)obj;
		}

		public static byte[] ToByteArray(this IEnumerable<byte[]> list, int size)
		{
			var bytes = new byte[size];
			var index = 0;
			foreach (var listb in list) {
				var left = size - index;
				var length = left < listb.Length ? left : listb.Length;
				Array.Copy(listb, 0, bytes, index, length);
			}
			return bytes;
		}
	}
}
