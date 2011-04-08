using System;

namespace Ketchup.Interfaces
{
	public interface ICommand
	{
		KetchupClient Client { get; set; }
		string Key { get; set; }
		string Bucket { get; set; }
		object State { get; set; }
		void Process(byte[] response, Exception exception);
	}
}
