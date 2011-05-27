using System;
using Ketchup.Config;
using Ketchup.Hashing;
using Ketchup.Interfaces;
using Ketchup.IO;
using Ketchup.Protocol.Exceptions;

namespace Ketchup.Protocol.Commands
{
	public class GetCommand<T> : ICommand
	{
		public Bucket Bucket { get; set; }
		public string Key { get; set; }
		public object State { get; set; }
		public Action<T, object> Hit { get; set; }
		public Action<object> Miss { get; set; }
		public Action<Exception, object> Error { get; set; }

		public GetCommand(Bucket bucket, string key)
		{
			Bucket = bucket;
			Key = key;
		}

		public Bucket Get()
		{
			var cmd = Miss == null ? Op.GetQ : Op.Get;
			var packet = new Packet<T>(cmd, Bucket.ModifiedKey(Key)).Serialize();
			var node = Hasher.GetNode(Bucket, Key);
			return Bucket.QueueOperation(node, packet, Process, Error, this);
		}

		public void Process(byte[] response, object command)
		{
			var cmd = (GetCommand<T>)command;
			try
			{
				var packet = new Packet<T>(response);
				if (cmd.Hit != null) cmd.Hit(packet.Value(), cmd.State);
			}
			catch (ProtocolException ex)
			{
				if (ex is NotFoundException)
					if (cmd.Miss != null) cmd.Miss(cmd.State);
			}
			catch (Exception ex)
			{
				if (cmd.Error != null) cmd.Error(ex, cmd.State);
			}
		}
	}
}
