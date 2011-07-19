using System;
using System.Collections.Generic;
using Ketchup.Protocol.Commands;
using Ketchup.Config;
using Ketchup.IO;

namespace Ketchup {
	public class Bucket {

		public KetchupClient Client { get; set; }
		public string Name { get; set; }
		public int Port { get; set; }
		public bool Prefix { get; set; }
		public IList<string> ConfigNodes { get; private set; }
		public IDictionary<string, string> Attributes { get; private set; }

		public Bucket() 
		{
			Name = "default";
			Port = 0;
			Prefix = true;
			ConfigNodes = new List<string>();
			Attributes = new Dictionary<string, string>();
		}

		public Bucket Operate(Node node, byte[] packet, Action<byte[], object> process, Action<Exception, object> error, object command) {
			var op = new Operation(node, packet, process, error, command);
			if(KetchupConfig.Current.UseEventLoop) 
				Client.QueueOperation(op);
			else op.BeginSend();
			return this;
		}

		public string ModifiedKey(string key)
		{
			return Prefix ? Name + "-" + key : key;
		}

		public string OriginalKey(string key)
		{
			return Prefix ? key.Substring(Name.Length + 1) : key;
		}
	}
}
