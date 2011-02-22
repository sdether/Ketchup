using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ketchup.Protocol {
	internal class Packet<T> {
		private byte[] _headerb = new byte[24];
		private byte[] _extrasb = new byte[0];
		private byte[] _keyb = new byte[0];
		private byte[] _valb;
		private T _value;

		public Packet() {
			_headerb[0] = (byte)Protocol.Magic.Request; //magic default
		}

		#region fluent

		public Packet<T> Magic(Magic magic) {
			_headerb[0] = (byte)magic;
			return this;
		}

		public Packet<T> Magic(out Magic magic) {
			magic = (Protocol.Magic)_headerb[0];
			return this;
		}

		public Packet<T> Operation(Op op) {
			_headerb[1] = (byte)op;
			return this;
		}

		public Packet<T> Operation(out Op op){
			op = (Protocol.Op)_headerb[1];
			return this;
		}

		public Packet<T> DataType(Data data) {
			_headerb[5] = (byte)data;
			return this;
		}

		public Packet<T> DataType(out Data data) {
			data = (Protocol.Data)_headerb[5];
			return this;
		}

		public Packet<T> Status(Response status) {
			((short)status).CopyTo(_headerb, 6);
			return this;
		}

		public Packet<T> Status(out Response status) {
			status = (Protocol.Response)_headerb.GetInt16(6);
			return this;
		}

		public Packet<T> Opaque(int value) {
			value.CopyTo(_headerb, 12);
			return this;
		}

		public Packet<T> Opaque(out int value) {
			value = _headerb.GetInt32(12);
			return this;
		}

		public Packet<T> CAS(long value) {
			value.CopyTo(_headerb, 16);
			return this;
		}

		public Packet<T> CAS(out long value) {
			value = _headerb.GetInt64(16);
			return this;
		}

		public Packet<T> Extras(byte[] extras) {
			_extrasb = extras;
			ExtraLength(Convert.ToInt16(extras.Length));
			return this;
		}

		public Packet<T> Extras(byte[] returnb, int index, short length) {
			Array.Copy(returnb, index, _extrasb, 0, length);
			return this;
		}

		public Packet<T> Extras(out byte[] extras) {
			extras = _extrasb;
			return this;
		}

		public Packet<T> Key(string key) {
			_keyb = Encoding.UTF8.GetBytes(key);
			KeyLength(Convert.ToInt16(key.Length));
			return this;
		}

		public Packet<T> Key(byte[] returnb, int index, short length) {
			Array.Copy(returnb, index, _keyb, 0, length);
			return this;
		}

		public Packet<T> Key(out string key) {
			key = Encoding.UTF8.GetString(_keyb);
			return this;
		}

		public Packet<T> Value(T value) {
			_value = value;
			return this;
		}

		public Packet<T> Value(byte[] returnb, int index, int length) {
			_valb = new byte[length];
			Array.Copy(returnb, index, _valb, 0, length);
			_value = _valb.GetObject<T>();
			return this;
		}

		public Packet<T> Value(out T value) {
			value = _value;
			return this;
		}

		public Packet<T> Header(byte[] returnb) {
			Array.Copy(returnb, 0, _headerb, 0, 24);
			return this;
		}

		#endregion

		public T Value() {
			var status = Response.NoError;
			var key = "";
			var op = Protocol.Op.Get;

			this.Status(out status)
				.Key(out key)
				.Operation(out op);

			var vals = Encoding.UTF8.GetString(_valb);

			switch (status) {
				case Response.NoError:
					return _value;
				case Response.KeyNotFound:
					throw new Protocol.NotFoundException(key, op, vals);
				case Response.AuthContinue:
					throw new Protocol.AuthContinueException(key, op, vals);
				case Response.AuthError:
					throw new Protocol.AuthErrorException(key, op, vals);
				case Response.Busy:
					throw new Protocol.BusyException(key, op, vals);
				case Response.IncrDecrNonNumeric:
					throw new Protocol.IncrDecrNonNumericException(key, op, vals);
				case Response.InternalError:
					throw new Protocol.InternalException(key, op, vals);
				case Response.InvalidArguments:
					throw new Protocol.InvalidArgumentException(key, op, vals);
				case Response.ItemNotStored:
					throw new Protocol.ItemNotStoredException(key, op, vals);
				case Response.KeyExists:
					throw new Protocol.KeyExistsException(key, op, vals);
				case Response.NotSupported:
					throw new Protocol.NotSupportedException(key, op, vals);
				case Response.OutOfMemory:
					throw new Protocol.OutOfMemoryException(key, op, vals);
				case Response.TemporaryFailure:
					throw new Protocol.TemporaryFailureException(key, op, vals);
				case Response.UnknownCommand:
					throw new Protocol.UnknownCommandException(key, op, vals);
				case Response.ValueTooLarge:
					throw new Protocol.ValueTooLargeException(key, op, vals);
				case Response.InvalidVBucketServer:
					throw new Protocol.InvalidVBucketServer(key, op, vals);
				default:
					throw new Protocol.UnknownResponseExcepton(key, op, (short)status, vals);
			}
			
		}

		public byte[] Serialize() {
			var valb = _value.GetBytes<T>();
			TotalLength(_extrasb.Length + _keyb.Length + valb.Length);

			var result = new List<byte>();
			result.AddRange(_headerb);
			result.AddRange(_extrasb);
			result.AddRange(_keyb);
			result.AddRange(valb);

			return result.ToArray();
		}

		public T Deserialize(byte[] returnb) {
			//create a new packet object with the same type T to house the response:
			short extral = 0;
			short keyl = 0;
			short headerl = 24;
			return new Packet<T>()
				.Header(returnb)
				.ExtraLength(out extral)
				.KeyLength(out keyl)
				.Extras(returnb, headerl, extral)
				.Key(returnb, headerl + extral, keyl)
				.Value(returnb, headerl + extral + keyl, (returnb.Length - (headerl + extral + keyl)))
				.Value();
		}

		private Packet<T> KeyLength(short length) {
			length.CopyTo(_headerb, 2);
			return this;
		}

		private Packet<T> KeyLength(out short length) {
			length = _headerb.GetInt16(2);
			return this;
		}

		private Packet<T> ExtraLength(short length) {
			length.CopyTo(_headerb, 4, 1);
			return this;
		}

		private Packet<T> ExtraLength(out short length) {
			length = Convert.ToInt16(_headerb[4]);
			return this;
		}

		private Packet<T> TotalLength(int length) {
			length.CopyTo(_headerb, 8);
			return this;
		}

		private Packet<T> TotalLength(out int length) {
			length = _headerb.GetInt32(8);
			return this;
		}
	}
}
