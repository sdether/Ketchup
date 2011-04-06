using System;
using System.Configuration;
using Ketchup.Config;
using Ketchup.IO;

namespace Ketchup {

	public class KetchupClient 
	{
		private readonly EventLoop loop = new EventLoop();

		public KetchupClient() 
		{
			var config = GetConfig();
			KetchupConfig.Init(config);
			loop.Start();
		}

		public KetchupClient(KetchupConfig config) 
		{
			KetchupConfig.Init(config);
			loop.Start();
		}

		public KetchupClient QueueOperation(Node node, byte[] packet, Action<byte[], object> process, Action<Exception, object> error, object state)
		{
			var op = new Operation(packet, node.NodeSocket, process, error, state);
			loop.QueueSend(op);
			return this;
		}

		private static KetchupConfig GetConfig()
		{
			var configSection = KetchupConfigSection.Current;
			if (configSection == null) throw new ConfigurationErrorsException(
				"Configuration missing. Either create a new KetchupConfig and call KetchupConfig.Init() or add a KetchupConfigSection to your config file.");

			return configSection.ToKetchupConfig();
		}
	}
}
