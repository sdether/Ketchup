using System;
using Ketchup.Hashing;
using Ketchup.Interfaces;

namespace Ketchup.Protocol.Commands
{
	public class FlushCommand : ICommand
	{
		public Bucket Bucket { get; set; }
		public object State { get; set; }
		public Action<object> Success { get; set; }
		public Action<Exception, object> Error { get; set; }

		public FlushCommand(Bucket bucket)
		{
			Bucket = bucket;
		}

		public Bucket Flush()
		{
			var cmd = Success == null ? Op.FlushQ : Op.Flush;
			var packet = new Packet<string>(cmd).Serialize();
			var node = Hasher.GetNode(Bucket, "1");
			return Bucket.Operate(node, packet, Process, Error, this);
		}

		public void Process(byte[] response, object command)
		{
			var cmd = (FlushCommand)command;
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
