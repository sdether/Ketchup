using System;
using System.Collections.Generic;
using System.Text;
using Ketchup.Protocol.Exceptions;

namespace Ketchup.Protocol {
	internal class Packet<T> {
		private const short headerl = 24;
		private readonly byte[] headerb = new byte[24];
		private bool hasval;
		private byte[] extrasb = new byte[0];
		private byte[] keyb = new byte[0];
		private byte[] valb;
		
		private T valueT;

		public Packet() {
			if (headerb != null) headerb[0] = (byte)Protocol.Magic.Request; //magic default
		}

		public Packet(Op operation) : this() {
			Operation(operation);
		}

		public Packet(Op operation, string key) : this(operation) {
			Key(key);
		}

		#region fluent

		public Packet<T> Magic(Magic magic) {
			headerb[0] = (byte)magic;
			return this;
		}

		public Packet<T> Magic(out Magic magic) {
			magic = (Magic)headerb[0];
			return this;
		}

		public Packet<T> Operation(Op op) {
			headerb[1] = (byte)op;
			return this;
		}

		public Packet<T> Operation(out Op op){
			op = (Op)headerb[1];
			return this;
		}

		public Packet<T> DataType(Data data) {
			headerb[5] = (byte)data;
			return this;
		}

		public Packet<T> DataType(out Data data) {
			data = (Data)headerb[5];
			return this;
		}

		public Packet<T> Status(Response status) {
			((short)status).CopyTo(headerb, 6);
			return this;
		}

		public Packet<T> Status(out Response status) {
			status = (Response)headerb.GetInt16(6);
			return this;
		}

		public Packet<T> Opaque(int opaque) {
			opaque.CopyTo(headerb, 12);
			return this;
		}

		public Packet<T> Opaque(out int opaque) {
			opaque = headerb.GetInt32(12);
			return this;
		}

		public Packet<T> CAS(long cas) {
			cas.CopyTo(headerb, 16);
			return this;
		}

		public Packet<T> CAS(out long cas) {
			cas = headerb.GetInt64(16);
			return this;
		}

		public Packet<T> Extras(byte[] extras) {
			extrasb = extras;
			ExtraLength(Convert.ToInt16(extras.Length));
			return this;
		}

		public Packet<T> Extras(byte[] returnb, int index, short length) {
			extrasb = new byte[length];
			Array.Copy(returnb, index, extrasb, 0, length);
			return this;
		}

		public Packet<T> Extras(out byte[] extras) {
			extras = extrasb;
			return this;
		}

		public Packet<T> Key(string key) {
			keyb = Encoding.UTF8.GetBytes(key);
			KeyLength(Convert.ToInt16(key.Length));
			return this;
		}

		public Packet<T> Key(byte[] returnb, int index, short length) {
			keyb = new byte[length];
			Array.Copy(returnb, index, keyb, 0, length);
			return this;
		}

		public Packet<T> Key(out string key) {
			key = Encoding.UTF8.GetString(keyb);
			return this;
		}

		public Packet<T> Value(T value) {
			valueT = value;
			hasval = true;
			return this;
		}

		public Packet<T> Value(byte[] returnb, int index, int length) {
			valb = new byte[length];
			Array.Copy(returnb, index, valb, 0, length);
			hasval = true;
			return this;
		}

		public Packet<T> Value(bool hasValue) {
			//some memcached protocol commands like incr MUST have no value, not even a 0, problematic for value-types;
			hasval = hasValue;
			return this;
		}

		public Packet<T> Value(out T value) {
			value = valueT;
			return this;
		}

		public Packet<T> Header(byte[] returnb) {
			Array.Copy(returnb, 0, headerb, 0, 24);
			return this;
		}

		#endregion

		public T Value() {
			Response status;
			string key;
			Op op;

			Status(out status)
				.Key(out key)
				.Operation(out op);

			var vals = Encoding.UTF8.GetString(valb);

			switch (status) {
				case Response.NoError:
					valueT = valb.GetObject<T>();
					return valueT;
				case Response.KeyNotFound:
					throw new NotFoundException(key, op, vals);
				case Response.AuthContinue:
					throw new AuthContinueException(key, op, vals);
				case Response.AuthError:
					throw new AuthErrorException(key, op, vals);
				case Response.Busy:
					throw new BusyException(key, op, vals);
				case Response.IncrDecrNonNumeric:
					throw new IncrDecrNonNumericException(key, op, vals);
				case Response.InternalError:
					throw new InternalException(key, op, vals);
				case Response.InvalidArguments:
					throw new InvalidArgumentException(key, op, vals);
				case Response.ItemNotStored:
					throw new ItemNotStoredException(key, op, vals);
				case Response.KeyExists:
					throw new KeyExistsException(key, op, vals);
				case Response.NotSupported:
					throw new OperationNotSupportedException(key, op, vals);
				case Response.OutOfMemory:
					throw new ServerOutOfMemoryException(key, op, vals);
				case Response.TemporaryFailure:
					throw new TemporaryFailureException(key, op, vals);
				case Response.UnknownCommand:
					throw new UnknownCommandException(key, op, vals);
				case Response.ValueTooLarge:
					throw new ValueTooLargeException(key, op, vals);
				case Response.InvalidVBucketServer:
					throw new InvalidVBucketServer(key, op, vals);
				default:
					throw new UnknownResponseExcepton(key, op, (short)status, vals);


			}
			
		}

		public byte[] Serialize() {
			var valueb = hasval ? valueT.GetBytes() : new byte[0];
			TotalLength(extrasb.Length + keyb.Length + valueb.Length);

			var result = new List<byte>();
			result.AddRange(headerb);
			result.AddRange(extrasb);
			result.AddRange(keyb);
			result.AddRange(valueb);

			return result.ToArray();
		}

		public T Deserialize(byte[] returnb) {
			//create a new packet object with the same type T to house the response:
			short extral;
			short keyl;
			int totall;

			return new Packet<T>()
				.Header(returnb)
				.ExtraLength(out extral)
				.KeyLength(out keyl)
				.TotalLength(out totall)
				.Extras(returnb, headerl, extral)
				.Key(returnb, headerl + extral, keyl)
				.Value(returnb, headerl + extral + keyl, (totall - (extral + keyl)))
				.Value();
		}

		private void KeyLength(short length) {
			length.CopyTo(headerb, 2);
			return;
		}

		private Packet<T> KeyLength(out short length) {
			length = headerb.GetInt16(2);
			return this;
		}

		private void ExtraLength(short length) {
			length.CopyTo(headerb, 4, 1);
			return;
		}

		private Packet<T> ExtraLength(out short length) {
			length = Convert.ToInt16(headerb[4]);
			return this;
		}

		private void TotalLength(int length) {
			length.CopyTo(headerb, 8);
			return;
		}

		private Packet<T> TotalLength(out int length) {
			length = headerb.GetInt32(8);
			return this;
		}
	}
}
