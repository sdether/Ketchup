using System;
using System.Configuration;
using Ketchup.Config;
using Ketchup.IO;

namespace Ketchup {

	public class KetchupClient 
	{
		private readonly EventLoop loop = new EventLoop();

		public Bucket DefaultBucket
		{
			get 
			{
				return KetchupConfig.Current.DefaultBucket;
			}	
		}


		public KetchupClient() 
		{
			var config = GetConfig();
			KetchupConfig.Init(config, this);
			loop.Start();
		}

		public KetchupClient(KetchupConfig config) 
		{
			config.Init(this);
			loop.Start();
		}

		public KetchupClient(string host, int port) 
		{
			var config = new KetchupConfig("default", host, port);
			config.Init(this);
		}

		public KetchupClient QueueOperation(Node node, byte[] packet, Action<byte[], object> process, Action<Exception, object> error, object state)
		{
			var op = new Operation(packet, node, process, error, state);
			loop.QueueSend(op);
			return this;
		}

		public Bucket GetBucket(string bucketName) 
		{
			return KetchupConfig.Current.GetBucket(bucketName);
		}

		private KetchupConfig GetConfig()
		{
			var configSection = KetchupConfigSection.Current;
			if (configSection == null) throw new ConfigurationErrorsException(
				"Configuration missing. Either create a new KetchupConfig and call KetchupConfig.Init() or add a KetchupConfigSection to your config file.");

			return configSection.ToKetchupConfig(this);
		}
	}
}
