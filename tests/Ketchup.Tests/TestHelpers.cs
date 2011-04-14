using System;
using System.Threading;
using Ketchup.Config;

namespace Ketchup.Tests {

	public static class TestHelpers
	{
		public static Bucket Bucket = new KetchupClient("locahost", 11211).DefaultBucket;
	}
}
