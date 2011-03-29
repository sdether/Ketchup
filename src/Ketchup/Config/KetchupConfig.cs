using System;
using System.Collections.Generic;
using System.Configuration;

namespace Ketchup.Config
{
	public class KetchupConfig
	{
		private static readonly object _sync = new object();
		private NodeList nodes;
		private IDictionary<string, IList<Node>> bucketNodes;
		private static KetchupConfig current;
		private readonly IDictionary<string, Bucket> buckets = new Dictionary<string, Bucket>();
		private readonly IList<string> configNodes = new List<string>();

		public static KetchupConfig Current
		{
			get
			{
				lock (_sync)
				{
					if (current == null && KetchupConfigSection.Current == null)
						throw new ConfigurationErrorsException("Configuration missing. Either create a new KetchupConfig and call KetchupConfig.Init() or add a KetchupConfigSection to your config file.");

					return current ?? (current = KetchupConfigSection.Current.ToKetchupConfig().Init());
				}
			}
		}

		#region settings

		/// <summary>
		/// if a node does not respond within this timespan, retry the node up to NodeConnectionMaxRetries, default is 500 milliseconds
		/// </summary>
		public TimeSpan ConnectionTimeout { get; set; }

		/// <summary>
		/// If the connection to a node fails, try again after this amount of time, default is 100 milliseconds
		/// </summary>
		public TimeSpan ConnectionRetryDelay { get; set; }

		/// <summary>
		/// If a node is marked "dead", this is the amount of time ketchup will wait to retry it, default is 1 second
		/// </summary>
		public TimeSpan DeadNodeRetryDelay { get; set; }

		/// <summary>
		/// if true, Ketchup will try to place the key in another node when a node is marked "dead", default is false
		/// </summary>
		public bool Failover { get; set; }

		/// <summary>
		/// if true, values over 1MB are compressed with gzip before being placed on server, default is true
		/// </summary>
		public bool Compression { get; set; }

		/// <summary>
		/// Number of times a node can fail before marking it "dead", default is 2
		/// </summary>
		public int ConnectionRetryCount { get; set; }

		/// <summary>
		/// hashing algorithm to use, default or ketama
		/// </summary>
		public HashingAlgortihm HashingAlgorithm { get; set; }

		#endregion

		public KetchupConfig()
		{
			Compression = true;
			Failover = false;
			ConnectionRetryDelay = new TimeSpan(0, 0, 0, 0, 100);
			ConnectionRetryCount = 2;
			ConnectionTimeout = new TimeSpan(0, 0, 0, 0, 500);
			DeadNodeRetryDelay = new TimeSpan(0, 0, 1);
			HashingAlgorithm = HashingAlgortihm.Default;
		}

		public KetchupConfig AddBucket(string name = "default", int port = 0, bool prefix = true)
		{
			return AddBucket(new Bucket
			{
				Name = name,
				Port = port,
				Prefix = prefix
			});
		}

		public KetchupConfig AddBucket(Bucket bucket)
		{
			buckets.Add(bucket.Name, bucket);
			return this;
		}


		public KetchupConfig AddNode(string endPoint)
		{
			configNodes.Add(endPoint);
			return this;
		}

		public Node GetNode(string address)
		{
			var node = nodes.GetById(address);
			if (node == null)
				throw new Exception("Host and port specified are not a valid node in the Ketchup Config");

			return node;
		}

		public IList<Node> GetNodes(string bucket)
		{
			return bucketNodes[bucket];
		}

		public string GetPrefixKey(string key, string bucket)
		{
			return buckets[bucket].Prefix ? bucket + "-" + key : key;
		}

		public string GetOriginalKey(string key, string bucket)
		{
			return buckets[bucket].Prefix ? key.Substring(bucket.Length + 1) : key;
		}

		public static KetchupConfig Init(KetchupConfig config)
		{
			config.Init();
			return config;
		}

		internal KetchupConfig Init()
		{
			bucketNodes = new Dictionary<string, IList<Node>>();
			nodes = new NodeList();

			foreach (var bucket in buckets.Values)
			{
				/* there are 3 options for buckets: nodes defined, port defined, all endpoints
				 * there are 2 options for nodes: ip defined, ip+port defined
				 */

				//first add it to the bucket hash and initialize the nodelist
				bucketNodes.Add(bucket.Name, new List<Node>());

				//option 1: nodes are defined, if port is defined, use port, otherwise use default port
				if (bucket.ConfigNodes.Count > 0)
				{
					InitSpecifiedNodes(bucket);
					continue;
				}

				//option 2: port is defined in the bucket list, use port with ips in node list
				if (bucket.Port > 0)
				{
					InitPortNodes(bucket);
					continue;
				}

				InitAllNodes(bucket);
			}

			current = this;
			return this;
		}

		private void InitPortNodes(Bucket bucket)
		{
			foreach (var cn in configNodes)
			{
				var host = cn.Split(':')[0];
				bucketNodes[bucket.Name].Add(
					nodes.GetOrCreate(host + ":" + bucket.Port)
				);
			}
		}

		private void InitAllNodes(Bucket bucket)
		{
			foreach (var cn in configNodes)
				bucketNodes[bucket.Name].Add(nodes.GetOrCreate(cn));
		}

		private void InitSpecifiedNodes(Bucket bucket)
		{
			foreach (var ep in bucket.ConfigNodes)
				bucketNodes[bucket.Name].Add(
					nodes.GetOrCreate(ep)
				);
		}
	}
}
