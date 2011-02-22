using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ketchup.Protocol {

	public enum Magic : byte {
		Request = 0x80,
		Response = 0x81
	}

	public enum Response : short {
		NoError = 0x0000,
		KeyNotFound = 0x0001,
		KeyExists = 0x0002,
		ValueTooLarge = 0x0003,
		InvalidArguments = 0x0004,
		ItemNotStored = 0x0005,
		IncrDecrNonNumeric = 0x0006,
		InvalidVBucketServer = 0x0007,
		AuthError = 0x0008,
		AuthContinue = 0x0009,
		UnknownCommand = 0x0081,
		OutOfMemory = 0x0082,
		NotSupported = 0x0083,
		InternalError = 0x0084,
		Busy = 0x0085,
		TemporaryFailure = 0x0086
	}

	public enum Op : byte {
		Get = 0x00,
		Set = 0x01,
		Add = 0x02,
		Replace = 0x03,
		Delete = 0x04,
		Incr = 0x05,
		Decr = 0x06,
		Quit = 0x07,
		Flush = 0x08,
		GetQ = 0x09,
		NoOp = 0x0a,
		Version = 0x0b,
		GetK = 0x0c,
		GetKQ = 0x0d,
		Append = 0x0e,
		Prepend = 0x0f,
		Stat = 0x10,
		SetQ = 0x11,
		AddQ = 0x12,
		ReplaceQ = 0x13,
		DeleteQ = 0x14,
		IncrQ = 0x15,
		DecrQ = 0x16,
		QuitQ = 0x17,
		FlushQ = 0x18,
		AppendQ = 0x19,
		PrependQ = 0x1a,
		SASLListMechs = 0x20,
		SASLAuth = 0x21,
		SASLStep = 0x22,
		RGet = 0x30,
		RSet = 0x31,
		RSetQ = 0x32,
		RAppend = 0x33,
		RAppendQ = 0x34,
		RPrepend = 0x35,
		RPrependQ = 0x36,
		RDelete = 0x37,
		RDeleteQ = 0x38,
		RIncr = 0x39,
		RIncrQ = 0x3a,
		RDecr = 0x3b,
		RDecrQ = 0x3c,
		TAPConnect = 0x40,
		TAPMutation = 0x41,
		TAPDelete = 0x42,
		TAPFlush = 0x43,
		TAPOpaque = 0x44,
		TAPVBucketSet = 0x45
	}

	public enum Data : byte {
		Raw = 0x00
	}
}
