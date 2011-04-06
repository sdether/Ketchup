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
		public string Bucket { get; set; }
		public object State { get; set; }
		public Action<T, object> Hit { get; set; }
		public Action<object> Miss { get; set; }
		public Action<Exception, object> Error { get; set; }

		public GetCommand(KetchupClient client, string key, string bucket) {
			Client = client;
			Key = key;
			Bucket = bucket;
		}

		public KetchupClient Get()
		{
			var cmd = Miss == null ? Op.GetQ : Op.Get;
			var packet = new Packet<T>(cmd, config.GetPrefixKey(Key, Bucket)).Serialize();
			return Client.Queue(new Operation(packet, Key, Bucket, Process, Error, this));
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
