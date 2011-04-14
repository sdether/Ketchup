using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Ketchup.Protocol.Exceptions;

namespace Ketchup.Protocol
{
	internal class PacketHeader
	{
		private const short headerl = 24;
		private byte[] headerb = new byte[24];

		public PacketHeader()
		{
			if (headerb != null) headerb[0] = (byte)Magic.Request; //magic default
		}

		public PacketHeader(byte[] returnb)
		{
			Array.Copy(returnb, 0, headerb, 0, 24);
		}

		public PacketHeader(IEnumerable<byte> returnb) 
		{
			headerb = returnb.Take(headerl).ToArray();
		}

		public Magic Magic
		{
			get { return (Magic)headerb[0]; }
			set { headerb[0] = (byte)value; }
		}

		public Op Command
		{
			get { return (Op)headerb[1]; }
			set { headerb[1] = (byte)value; }
		}

		public Data DataType
		{
			get { return (Data)headerb[5]; }
			set { headerb[5] = (byte)value; }
		}

		public Response Status
		{
			get { return (Response)headerb.GetInt16(6); }
			set { ((short)value).CopyTo(headerb, 6); }
		}

		public int Opaque
		{
			get { return headerb.GetInt32(12); }
			set { value.CopyTo(headerb, 12); }
		}

		public long CAS
		{
			get { return headerb.GetInt64(16); }
			set { value.CopyTo(headerb, 16); }
		}

		public byte[] Bytes
		{
			get { return headerb; }
			set { headerb = value; }
		}

		public short KeyLength
		{
			get { return headerb.GetInt16(2); }
			set { value.CopyTo(headerb, 2); }
		}

		public short ExtraLength
		{
			get { return Convert.ToInt16(headerb[4]); }
			set { value.CopyTo(headerb, 4, 1); }
		}

		public int PacketLength
		{
			get { return headerl + TotalLength; }
		}

		public int TotalLength
		{
			get { return headerb.GetInt32(8); }
			set { value.CopyTo(headerb, 8); }
		}
	}

	internal class Packet<T>
	{
		private readonly PacketHeader header;
		private bool hasval;
		private byte[] extrasb = new byte[0];
		private byte[] keyb = new byte[0];
		private byte[] valb;

		private T valueT;

		public Packet(Op command)
		{
			header = new PacketHeader {Command = command};
		}

		public Packet(Op command, string key)
			: this(command)
		{
			Key(key);
		}

		public Packet(byte[] returnb) {
			header = new PacketHeader(returnb);
			var headerl = header.Bytes.Length;
			var extral = header.ExtraLength;
			var keyl = header.KeyLength;
			var totall = header.TotalLength;
			Extras(returnb, headerl, extral)
				.Key(returnb, headerl + extral, keyl)
				.Value(returnb, headerl + extral + keyl, (totall - (extral + keyl)));
		}

		public Packet<T> Extras(byte[] extras)
		{
			extrasb = extras;
			header.ExtraLength = Convert.ToInt16(extras.Length);
			return this;
		}

		public Packet<T> Extras(byte[] returnb, int index, short length)
		{
			extrasb = new byte[length];
			Array.Copy(returnb, index, extrasb, 0, length);
			return this;
		}

		public Packet<T> Extras(out byte[] extras)
		{
			extras = extrasb;
			return this;
		}

		public Packet<T> Key(string key)
		{
			keyb = Encoding.UTF8.GetBytes(key);
			header.KeyLength = Convert.ToInt16(key.Length);
			return this;
		}

		public Packet<T> Key(byte[] returnb, int index, short length)
		{
			keyb = new byte[length];
			Array.Copy(returnb, index, keyb, 0, length);
			return this;
		}

		public Packet<T> Key(out string key)
		{
			key = Encoding.UTF8.GetString(keyb);
			return this;
		}

		public Packet<T> Value(T value)
		{
			valueT = value;
			hasval = true;
			return this;
		}

		public Packet<T> Value(byte[] returnb, int index, int length)
		{
			valb = new byte[length];
			Array.Copy(returnb, index, valb, 0, length);
			hasval = true;
			return this;
		}

		public Packet<T> Value(bool hasValue)
		{
			//some memcached protocol commands like incr MUST have no value, not even a 0, problematic for value-types;
			hasval = hasValue;
			return this;
		}

		public Packet<T> Value(out T value)
		{
			value = valueT;
			return this;
		}

		public T Value()
		{
			var status = header.Status;
			var cmd = header.Command;
			string key;
			Key(out key);

			var vals = Encoding.UTF8.GetString(valb);
			switch (status)
			{
				case Response.NoError:
					valueT = valb.GetObject<T>();
					return valueT;
				case Response.KeyNotFound:
					throw new NotFoundException(key, cmd, vals);
				case Response.AuthContinue:
					throw new AuthContinueException(key, cmd, vals);
				case Response.AuthError:
					throw new AuthErrorException(key, cmd, vals);
				case Response.Busy:
					throw new BusyException(key, cmd, vals);
				case Response.IncrDecrNonNumeric:
					throw new IncrDecrNonNumericException(key, cmd, vals);
				case Response.InternalError:
					throw new InternalException(key, cmd, vals);
				case Response.InvalidArguments:
					throw new InvalidArgumentException(key, cmd, vals);
				case Response.ItemNotStored:
					throw new ItemNotStoredException(key, cmd, vals);
				case Response.KeyExists:
					throw new KeyExistsException(key, cmd, vals);
				case Response.NotSupported:
					throw new OperationNotSupportedException(key, cmd, vals);
				case Response.OutOfMemory:
					throw new ServerOutOfMemoryException(key, cmd, vals);
				case Response.TemporaryFailure:
					throw new TemporaryFailureException(key, cmd, vals);
				case Response.UnknownCommand:
					throw new UnknownCommandException(key, cmd, vals);
				case Response.ValueTooLarge:
					throw new ValueTooLargeException(key, cmd, vals);
				case Response.InvalidVBucketServer:
					throw new InvalidVBucketServer(key, cmd, vals);
				default:
					throw new UnknownResponseExcepton(key, cmd, (short)status, vals);
			}

		}

		public byte[] Serialize()
		{
			var valueb = hasval ? valueT.GetBytes() : new byte[0];
			header.TotalLength = extrasb.Length + keyb.Length + valueb.Length;

			var result = new List<byte>();
			result.AddRange(header.Bytes);
			result.AddRange(extrasb);
			result.AddRange(keyb);
			result.AddRange(valueb);
			return result.ToArray();
		}
	}
}
