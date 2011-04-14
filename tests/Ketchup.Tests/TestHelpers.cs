using System;
using System.Threading;
using Ketchup.Config;

namespace Ketchup.Tests {

	public static class TestHelpers
	{
		public static Bucket Bucket = new KetchupClient("172.17.6.201", 11211).DefaultBucket;
	}
}
