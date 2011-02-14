using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;

namespace Ketchup.Config {
	public class KetchupConfigSectionHandler : IConfigurationSectionHandler {
		private KetchupConfig config = new KetchupConfig();
		public object Create(object parent, object configContext, XmlNode section) {
			bool hasBuckets = false;

			foreach (XmlNode sub in section.ChildNodes)
				switch (sub.Name) {
					case "Buckets":
						ParseBuckets(sub);
						break;
					case "Nodes":
						ParseNodes(sub);
						break;
					case "Settings":
						ParseSettings(sub);
						break;
				}

			if (!hasBuckets) {
				config.AddBucket(new Bucket());
					
			}

			return config;
		}
		private void ParseBuckets(XmlNode buckets) {

			foreach (XmlNode bx in buckets.ChildNodes) {
				var bucket = new Bucket();

				foreach(XmlAttribute at in bx.Attributes) {
					switch(at.Name){
						case "name":
							bucket.Name = at.Value;
							break;
						case "prefix":
							var prefix = true;
							if(!bool.TryParse(at.Value, out prefix)) 
								throw new ConfigurationErrorsException("Bucket attribute 'prefix' was not a valid boolean value, use 'true' or 'false' or leave blank, 'true' is the default", at);
							bucket.Prefix = prefix;
							break;
						case "port":
							int port = 0;
							if(!int.TryParse(at.Value,out port))
								throw new ConfigurationErrorsException("Bucket " + at.Name + " 'port' attribute was not a valid integer, use 0-65535", at);
							bucket.Port = port;
							break;
						default:
							bucket.Attributes.Add(at.Name, at.Value);
							break;
					}
				}

				//if the bucket defines explicit nodes (not recommended) 
				foreach (XmlNode nd in bx.ChildNodes) {
					foreach (XmlAttribute at in bx.Attributes) {
						switch (at.Name) {
							case "endpoint":
								bucket.ConfigNodes.Add(at.Value);
								break;
						}
					}
				}

				config.AddBucket(bucket);
			}
		}
		private void ParseSettings(XmlNode settings) {
			foreach (XmlNode setting in settings.ChildNodes) {
				foreach (XmlAttribute sa in setting.Attributes) {
					int sh;
					bool bo;
					switch (sa.Name) {
						case "compression":
							if (!bool.TryParse(sa.Value, out bo))
								throw new ConfigurationErrorsException("Compression setting was not a valid boolean, use 'true' or 'false'", setting);

							config.Compression = bo;
							break;
						case "failover":
							if (!bool.TryParse(sa.Value, out bo))
								throw new ConfigurationErrorsException("Failover setting was not a valid boolean, use 'true' or 'false'", setting);

							config.Failover = bo;
							break;
						case "connectionRetryDelay":
							if (!int.TryParse(sa.Value, out sh))
								throw new ConfigurationErrorsException("ConnectionRetryDelay was not a valid integer in milliseconds", setting);

							config.ConnectionRetryDelay = new TimeSpan(0, 0, 0, 0, sh);
							break;
						case "connectionRetryCount":
							if (!int.TryParse(sa.Value, out sh))
								throw new ConfigurationErrorsException("ConnectionRetryCount was not a valid integer", setting);

							config.ConnectionRetryCount = sh;
							break;
						case "connectionTimeout":
							if(!int.TryParse(sa.Value, out sh))
								throw new ConfigurationErrorsException("ConnectionTimeout was not a valid integer in millisecods", setting);

							config.ConnectionTimeout = new TimeSpan(0,0,0,0,sh);
							break;
						case "deadNodeRetryDelay":
							if (!int.TryParse(sa.Value, out sh))
								throw new ConfigurationErrorsException("DeadNodeRetryDelay was not a valid int integer in miliseconds", setting);

							config.DeadNodeRetryDelay = new TimeSpan(0,0,0,0,sh);
							break;
					}
				}
			}
		}
		private void ParseNodes (XmlNode nodes) {
			foreach(XmlNode node in nodes.ChildNodes) {
				foreach (XmlAttribute na in node.Attributes) {
					switch (na.Name) {
						case "endpoint":
							config.AddNode(na.Value);
							break;
					}
				}
			}
		}
	}
}
