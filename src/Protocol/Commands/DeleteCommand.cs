using System;
using Ketchup.Hashing;
using Ketchup.Interfaces;

namespace Ketchup.Protocol.Commands
{
	public class DeleteCommand : ICommand
	{
		public Bucket Bucket { get; set; }
		public string Key { get; set; }
		public object State { get; set; }
		public Action<object> Success { get; set; }
		public Action<Exception, object> Error { get; set; }

		public DeleteCommand(Bucket bucket, string key)
		{
			Bucket = bucket;
			Key = key;
		}

		public Bucket Delete()
		{
			var cmd = Success == null ? Op.DeleteQ : Op.Delete;
			var packet = new Packet<string>(cmd, Bucket.ModifiedKey(Key)).Serialize();
			var node = Hasher.GetNode(Bucket, Key);
			return Bucket.QueueOperation(node, packet, Process, Error, this);
		}

		public void Process(byte[] response, object command)
		{
			var cmd = (DeleteCommand)command;
			try
			{
				new Packet<object>(response).Value();
				if (cmd.Success != null) cmd.Success(cmd.State);
			}
			catch (Exception ex)
			{
				if (cmd.Error != null) cmd.Error(ex, cmd.State);
			}
		}

	}
}
