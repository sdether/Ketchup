using System;

namespace Ketchup.Interfaces
{
	public interface ICommand
	{
		Bucket Bucket { get; set; }
		object State { get; set; }
		Action<Exception, object> Error { get; set; }		
		void Process(byte[] response, object command);
	}
}
