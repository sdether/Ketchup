﻿using System;
using Ketchup.Config;
using Ketchup.Hashing;

namespace Ketchup.Protocol.Commands
{
	public class SetAddReplaceCommand<T>
	{
		private static readonly KetchupConfig config = KetchupConfig.Current;

		public KetchupClient Client { get; set; }
		public string Key { get; set; }
		public T Value { get; set; }
		public int Expiration { get; set; }
		public object State { get; set; }
		public Action<object> Success { get; set; }
		public Action<Exception, object> Error { get; set; }

		public SetAddReplaceCommand(string key, T value)
		{
			Key = key;
			Value = value;
		}

		public KetchupClient SetAddReplace(Op operation)
		{
			var extras = new byte[8];
			Expiration.CopyTo(extras, 4);
			var packet = new Packet<T>(operation, config.GetPrefixKey(Key, Client.Bucket)).Extras(extras).Value(Value);
			Hasher.GetNode(Key, Client.Bucket).Request(packet.Serialize(), Process, Error, this);
			return Client;
		}

		public static void Process(byte[] response, object Operation)
		{
			var op = (SetAddReplaceCommand<T>)Operation;
			try
			{
				new Packet<T>().Deserialize(response);
				if (op.Success != null) op.Success(op.State);
			}
			catch (Exception ex)
			{
				if (op.Error != null) op.Error(ex, op.State);
			}
		}

		public KetchupClient Set()
		{
			var op = Success == null ? Op.SetQ : Op.Set;
			return SetAddReplace(op);
		}
		public KetchupClient Add()
		{
			var op = Success == null ? Op.AddQ : Op.Add;
			return SetAddReplace(op);
		}
		public KetchupClient Replace()
		{
			var op = Success == null ? Op.ReplaceQ : Op.Replace;
			return SetAddReplace(op);
		}
	}


}