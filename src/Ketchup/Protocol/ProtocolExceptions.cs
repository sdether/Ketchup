using System;

namespace Ketchup.Protocol {
	public abstract class ProtocolException : ApplicationException {
		public string Key { get; private set; }
		public Op Operation { get; private set; }

		protected ProtocolException(string message, string key, Op op, string server)
			: base(string.Format("{0}\nServer Response: {1}\nKey: {2}\nOperation: {3}",message,server,key,op)) {
				Key = key;
				Operation = op;
		}
	}

	public class AuthContinueException : ProtocolException {
		public AuthContinueException(string key, Op op, string server) :
			base("Authentication continue response was returned from the server.", key, op, server) { }
	}

	public class AuthErrorException : ProtocolException {
		public AuthErrorException(string key, Op op, string server) :
			base("Authentication error response was returned from the server.", key, op, server) { }
	}

	public class BusyException : ProtocolException {
		public BusyException(string key, Op op, string server) :
			base("Server too busy.", key, op, server) { }
	}

	public class IncrDecrNonNumericException : ProtocolException {
		public IncrDecrNonNumericException(string key, Op op, string server) :
			base("Attempt to call Increment or Decrement on a non-numeric value.", key, op, server) { }
	}

	public class InternalException : ProtocolException {
		public InternalException(string key, Op op, string server) :
			base("Internal error.", key, op, server) { }
	}

	public class InvalidArgumentException : ProtocolException {
		public InvalidArgumentException(string key, Op op, string server) :
			base("Arguments passed to server were invalid.", key, op, server) { }
	}

	public class ItemNotStoredException : ProtocolException {
		public ItemNotStoredException(string key, Op op, string server) :
			base("The item was not stored on the server", key, op, server) { }
	}

	public class KeyExistsException : ProtocolException {
		public KeyExistsException(string key, Op op, string server) :
			base("An attempt was made to add a key that already exists", key, op, server) { }
	}

	public class NotFoundException : ProtocolException {
		public NotFoundException(string key, Op op, string server) :
			base("The specified key was not found on the server", key, op, server) { }
	}

	public class NotSupportedException : ProtocolException {
		public NotSupportedException(string key, Op op, string server) :
			base("The operation is not supported", key, op, server) { }
	}

	public class OutOfMemoryException : ProtocolException {
		public OutOfMemoryException(string key, Op op, string server) :
			base("The server is out of memory", key, op, server) { }
	}

	public class TemporaryFailureException : ProtocolException {
		public TemporaryFailureException(string key, Op op, string server) :
			base("The server had a temporary failure", key, op, server) { }
	}

	public class UnknownCommandException : ProtocolException {
		public UnknownCommandException(string key, Op op, string server) :
			base("An unknown command was sent to the server", key, op, server) { }
	}

	public class ValueTooLargeException : ProtocolException {
		public ValueTooLargeException(string key, Op op, string server) :
			base("The value passed to the server was too large", key, op, server) { }
	}

	public class InvalidVBucketServer : ProtocolException {
		public InvalidVBucketServer(string key, Op op, string server) :
			base("The vbucket specified belongs to another server", key, op, server) { }
	}

	public class UnknownResponseExcepton : ProtocolException {
		public UnknownResponseExcepton(string key, Op op, short status, string server) :
			base("The Response Status code was not recognized.\nResponse: " + status, key, op, server) { }
	}
}
