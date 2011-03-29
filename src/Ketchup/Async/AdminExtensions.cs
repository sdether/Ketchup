using System;
using Ketchup.Protocol;
using Ketchup.Config;

namespace Ketchup.Async {

	public static class AdminExtensions {

		//public static KetchupClient Version(this KetchupClient client, string address,
		//    Action<string> success, Action<Exception> error) {
		//    var node = KetchupConfig.Current.GetNode(address);
		//    Operations.Version(Op.Version, node, success, error);
		//    return client;
		//}

		//public static KetchupClient NoOp(this KetchupClient client, string address,
		//    Action success, Action<Exception> error) {
		//    var node = KetchupConfig.Current.GetNode(address);
		//    Operations.QuitNoOp(Op.NoOp, node, success, error);
		//    return client;
		//}

		//public static KetchupClient Quit(this KetchupClient client, string address,
		//    Action success, Action<Exception> error) {
		//    var node = KetchupConfig.Current.GetNode(address);
		//    var op = success == null ? Op.QuitQ : Op.Quit;
		//    Operations.QuitNoOp(op, node, success, error);
		//    return client;
		//}
	}
}

