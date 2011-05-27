using System;
using Ketchup.Config;
using Ketchup.Hashing;
using Ketchup.Interfaces;

namespace Ketchup.Protocol.Commands
{
	public class SetAddReplaceCommand<T> : ICommand
	{
		public Bucket Bucket { get; set; }
		public string Key { get; set; }
		public T Value { get; set; }
		public int Expiration { get; set; }
		public object State { get; set; }
		public Action<object> Success { get; set; }
		public Action<Exception, object> Error { get; set; }

		public SetAddReplaceCommand(Bucket bucket, string key, T value)
		{
			Bucket = bucket;
			Key = key;
			Value = value;
			Bucket = bucket;
		}

		public Bucket SetAddReplace(Op opcode)
		{
			var extras = new byte[8];
			Expiration.CopyTo(extras, 4);
			var packet = new Packet<T>(opcode, Bucket.ModifiedKey(Key)).Extras(extras).Value(Value).Serialize();
			var node = Hasher.GetNode(Bucket, Key);
			return Bucket.QueueOperation(node, packet, Process, Error, this);
		}

		public void Process(byte[] response, object command)
		{
			var cmd = (SetAddReplaceCommand<T>)command;
			try
			{
				new Packet<T>(response).Value();
				if (cmd.Success != null) cmd.Success(cmd.State);
			}
			catch (Exception ex)
			{
				if (cmd.Error != null) cmd.Error(ex, cmd.State);
			}
		}

		public Bucket Set()
		{
			var op = Success == null ? Op.SetQ : Op.Set;
			return SetAddReplace(op);
		}
		public Bucket Add()
		{
			var op = Success == null ? Op.AddQ : Op.Add;
			return SetAddReplace(op);
		}
		public Bucket Replace()
		{
			var op = Success == null ? Op.ReplaceQ : Op.Replace;
			return SetAddReplace(op);
		}
	}


}
