using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ketchup {
	public class KetchupClient {

		/// <summary>
		/// Creates a new instance of the ketchup client with no configuration
		/// </summary>
		public KetchupClient() { }

		public KetchupClient Get<T>(string key, Action<T, ushort> success, Action<Exception> error) {
			try {
				//Protocol.Get(key, success);
			} catch (Exception ex) {
				error(ex);
			}

			return this;
		}

		public KetchupClient Get(string key, Action<ArraySegment<byte>, ushort> success, Action<Exception> error) {
			throw new NotImplementedException();
		}

		public T Get<T>(string key)	{
			throw new NotImplementedException();
		}

		public object Get(string key){
			throw new NotImplementedException();
		}

		public KetchupClient Set<T>(string key, T value, DateTime expires,
			Action success,
			Action<Exception> error) {
			throw new NotImplementedException();
		}

		public KetchupClient Set(string key, ArraySegment<byte> data, DateTime expires,
			Action success,
			Action<Exception> error
			) {
			throw new NotImplementedException();
		}

		public KetchupClient Set(string key, ArraySegment<byte> data) {
			throw new NotImplementedException();
		}

		public KetchupClient Set<T>(string key, T data) {
			throw new NotImplementedException();
		}

		public KetchupClient Set(string key, object data) {
			throw new NotImplementedException();
		}
	}
}
