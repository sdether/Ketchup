using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ketchup.Protocol {
	internal class Packet {
		private IList<byte> _header = new List<byte>(24);
		private IList<byte> _extras;
		private IList<byte> _key;
		private IList<byte> _value;

		public Packet(){
			_header[0] = 0x80; //magic default
		}

		public Packet Magic(Magic magic){
			_header[0] = (byte)magic;
			return this;
		}

		public Packet Op(Op op){
			_header[1] = (byte)op;
			return this;
		}

		private Packet KeyLength(short length){
			var kbl = BitConverter.GetBytes(length);
			_header[2] = kbl[0];
			_header[3] = kbl[1];
			return this;
		}

		private Packet ExtraLength(short length){
			_header[4] = BitConverter.GetBytes(length)[1];
			return this;
		}

		public Packet DataType(Data data){
			_header[5] = (byte)data;
			return this;
		}

		public Packet Status(short status){
			var kbl = BitConverter.GetBytes(status);
			_header[6] = kbl[0];
			_header[7] = kbl[1];
			return this;
		}

		private Packet TotalLength(int length){
			var kbl = BitConverter.GetBytes(length);
			for(var i = 0;i<kbl.Length;i++)
				_header[i+8] = kbl[i];
			
			return this;
		}

		public Packet Opaque(int value){
			var kbl = BitConverter.GetBytes(value);
			for(var i = 0;i<kbl.Length;i++)
				_header[i+12] = kbl[i];
			
			return this;
		}

		public Packet CAS(long value){
			var kbl = BitConverter.GetBytes(value);
			for(var i = 0;i<kbl.Length;i++)
				_header[i+16] = kbl[i];
			
			return this;
		}

		public Packet Extras(byte[] extras){
			_extras = extras;
			ExtraLength(Convert.ToInt16(extras.Length));
			return this;
		}

		public Packet Key(string key){
			_key = Encoding.UTF8.GetBytes(key);
			KeyLength(Convert.ToInt16(key.Length));
			return this;
		}

		public Packet Value(byte[] value){
			_value = value;
			TotalLength(_extras.Count + _key.Count + _value.Count);
			return this;
		}

		public byte[] Create(){
			var result = new List<byte>();
			result.AddRange(_header);
			result.AddRange(_extras);
			result.AddRange(_key);
			result.AddRange(_value);
			
			return result.ToArray();
		}
	}
}
