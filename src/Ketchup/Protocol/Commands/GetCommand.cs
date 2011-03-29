using System;
using Ketchup.Config;
using Ketchup.Hashing;
using Ketchup.Protocol.Exceptions;

namespace Ketchup.Protocol.Commands
{
	public class GetCommand<T>
	{
		private static readonly KetchupConfig config = KetchupConfig.Current;

		public KetchupClient Client { get; set; }
		public string Key { get; set; }
		public object State { get; set; }
		public Action<T, object> Hit { get; set; }
		public Action<object> Miss { get; set; }
		public Action<Exception, object> Error { get; set; }

		public GetCommand(string key) {
			Key = key;
		}

		public KetchupClient Get()
		{
			var op = Miss == null ? Op.GetQ : Op.Get;
			var packet = new Packet<T>(op, config.GetPrefixKey(Key, Client.Bucket));
			Hasher.GetNode(Key, Client.Bucket).Request(packet.Serialize(), Process, Error, this);
			return Client;
		}

		private static void Process(byte[] response, object command)
		{
			var op = (GetCommand<T>)command;
			try
			{
				var packet = new Packet<T>(response);
				op.Hit(packet.Value(), op.State);
			}
			catch (ProtocolException ex)
			{
				if (ex is NotFoundException)
					if (op.Miss != null) op.Miss(op.State);
			}
		}

	}

	
}
