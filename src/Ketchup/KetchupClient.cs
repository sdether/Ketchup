using System.Configuration;
using System.Threading;
using Ketchup.Config;

namespace Ketchup {

	public class KetchupClient 
	{
		private readonly EventLoop loop = new EventLoop();

		public KetchupClient() 
		{
			var config = GetConfig();
			KetchupConfig.Init(config);
			StartThread();
		}

		public KetchupClient(KetchupConfig config) 
		{
			KetchupConfig.Init(config);
			StartThread();
		}

		public KetchupClient Queue(Operation op) {
			loop.QueueSend(op);
			return this;
		}
		
		private void StartThread() {
			new Thread(EventLoop.Run) {
				IsBackground = true,
				Name = "KetchupClient.EventLoop"
			}.Start(loop);
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
