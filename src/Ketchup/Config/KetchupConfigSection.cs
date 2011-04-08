using System;
using System.Configuration;

namespace Ketchup.Config {
	public class KetchupConfigSection : ConfigurationSection {
		/// <summary>
		/// Singleton Instance
		/// </summary>
		/// <returns></returns>
		public static KetchupConfigSection Current {
			get {
				var section = ConfigurationManager.GetSection("ketchup") as KetchupConfigSection;
				return section;
			}
		}

		[ConfigurationProperty("nodes")]
		[ConfigurationCollection(typeof(ConfigNode), AddItemName = "node")]
		public ConfigNodeCollection ConfigNodes {
			get {
				return (ConfigNodeCollection)this["nodes"] ?? new ConfigNodeCollection();
			}
		}

		[ConfigurationProperty("buckets")]
		[ConfigurationCollection(typeof(ConfigBucket), AddItemName = "bucket")]
		public ConfigBucketCollection ConfigBuckets {
			get {
				return (ConfigBucketCollection)this["buckets"] ?? new ConfigBucketCollection();
			}
		}
		
		[ConfigurationProperty("settings")]
		[ConfigurationCollection(typeof(ConfigSetting), AddItemName="setting")]
		public ConfigSettingCollection ConfigSettings {
			get {
				return (ConfigSettingCollection)this["settings"] ?? new ConfigSettingCollection();
			}
		}

		internal KetchupConfig ToKetchupConfig() {
			var config = new KetchupConfig();

			//nodes;
			foreach (ConfigNode cn in ConfigNodes)
				config.AddNode(cn.Host);

			//buckets;
			foreach (ConfigBucket cb in ConfigBuckets) {
				var bucket = new Bucket {
					Name = cb.Name,
					Port = cb.Port,
					Prefix = cb.Prefix
				};

				foreach (ConfigNode cbn in cb.ConfigNodes)
					bucket.ConfigNodes.Add(cbn.Host);

				config.AddBucket(bucket);
			}

			//settings;
			foreach (ConfigSetting sa in ConfigSettings) {
				int sh;
				bool bo;

				switch (sa.Name) {
					case "compression":
						if (!bool.TryParse(sa.Value, out bo))
							throw new ConfigurationErrorsException("Compression setting was not a valid boolean, use 'true' or 'false'");

						config.Compression = bo;
						break;
					case "failover":
						if (!bool.TryParse(sa.Value, out bo))
							throw new ConfigurationErrorsException("Failover setting was not a valid boolean, use 'true' or 'false'");

						config.Failover = bo;
						break;
					case "connectionRetryDelay":
						if (!int.TryParse(sa.Value, out sh))
							throw new ConfigurationErrorsException("ConnectionRetryDelay was not a valid integer in milliseconds");

						config.ConnectionRetryDelay = new TimeSpan(0, 0, 0, 0, sh);
						break;
					case "connectionRetryCount":
						if (!int.TryParse(sa.Value, out sh))
							throw new ConfigurationErrorsException("ConnectionRetryCount was not a valid integer");

						config.ConnectionRetryCount = sh;
						break;
					case "connectionTimeout":
						if (!int.TryParse(sa.Value, out sh))
							throw new ConfigurationErrorsException("ConnectionTimeout was not a valid integer in millisecods");

						config.ConnectionTimeout = new TimeSpan(0, 0, 0, 0, sh);
						break;
					case "deadNodeRetryDelay":
						if (!int.TryParse(sa.Value, out sh))
							throw new ConfigurationErrorsException("DeadNodeRetryDelay was not a valid int integer in miliseconds");

						config.DeadNodeRetryDelay = new TimeSpan(0, 0, 0, 0, sh);
						break;
				}
			}

			return config;
		}
	}

	public class ConfigNodeCollection : ConfigurationElementCollection {
		public ConfigNode this[int index] {
			get {
				return BaseGet(index) as ConfigNode;
			}
			set {
				if (BaseGet(index) != null)
					BaseRemoveAt(index);
				BaseAdd(index, value);
			}
		}

		protected override ConfigurationElement CreateNewElement(){
			return new ConfigNode();
		}

		protected override object GetElementKey(ConfigurationElement element) {
			return ((ConfigNode)element).Host;
		}
	}

	public class ConfigNode : ConfigurationElement {
		[ConfigurationProperty("host", IsRequired=true, IsKey=true)]
		public string Host {
			get {
				return this["host"] as string;
			}
		}
	}

	public class ConfigBucketCollection : ConfigurationElementCollection {
		public ConfigBucket this[int index] {
			get {
				return BaseGet(index) as ConfigBucket;
			}
			set {
				if (BaseGet(index) != null)
					BaseRemoveAt(index);
				BaseAdd(index, value);
			}
		}

		protected override ConfigurationElement CreateNewElement() {
			return new ConfigBucket();
		}

		protected override object GetElementKey(ConfigurationElement element) {
			return ((ConfigBucket)element).Name;
		}
	}

	public class ConfigBucket : ConfigurationElement {

		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name {
			get {
				return this["name"] as string;
			}
		}

		[ConfigurationProperty("port", DefaultValue = 0)]
		public int Port {
			get {
				return (int)this["port"];
			}
		}

		[ConfigurationProperty("prefix", DefaultValue = true)]
		public bool Prefix {
			get {
				return (bool)this["prefix"];
			}
		}

		[ConfigurationProperty("nodes", IsDefaultCollection=true)]
		[ConfigurationCollection(typeof(ConfigNode), AddItemName = "node")]
		public ConfigNodeCollection ConfigNodes {
			get {
				return (ConfigNodeCollection)this["nodes"] ?? new ConfigNodeCollection();
			}
		}
	}

	public class ConfigSettingCollection : ConfigurationElementCollection {
		public ConfigSetting this[int index] {
			get {
				return BaseGet(index) as ConfigSetting;
			}
			set {
				if (BaseGet(index) != null)
					BaseRemoveAt(index);
				BaseAdd(index, value);
			}
		}

		protected override ConfigurationElement CreateNewElement() {
			return new ConfigSetting();
		}

		protected override object GetElementKey(ConfigurationElement element) {
			return ((ConfigSetting)element).Name;
		}
	}

	public class ConfigSetting : ConfigurationElement {
		[ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name {
			get {
				return this["name"] as string;
			}
		}

		[ConfigurationProperty("value", IsRequired=true)]
		public string Value {
			get {
				return this["value"] as string;
			}
		}
	}
}
