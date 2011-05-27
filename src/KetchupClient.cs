using System;
using System.Configuration;
using Ketchup.Config;
using Ketchup.IO;

namespace Ketchup {

	public class KetchupClient 
	{
		public Bucket DefaultBucket
		{
			get 
			{
				return KetchupConfig.Current.DefaultBucket;
			}	
		}

		public KetchupClient(KetchupConfig config)
		{
			config.Init(this);
		}

		public KetchupClient()
			: this (GetConfig())
		{
		}

		public KetchupClient(string host, int port) 
			:this(host + ":" + port)
		{
		}

		public KetchupClient(string endpoint)
			: this(new KetchupConfig().AddNode(endpoint).AddBucket())
		{
		}


		public Bucket GetBucket(string bucketName) 
		{
			return KetchupConfig.Current.GetBucket(bucketName);
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
