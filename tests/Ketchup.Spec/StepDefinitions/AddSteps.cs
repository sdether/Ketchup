using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
using Ketchup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ketchup.Spec.Steps {
	[Binding]
	public class AddSteps {

		KetchupClient ketchup;

		[Given(@"a new Ketchup client")]
		public void GivenANewKetchupClient() {
			ketchup = new KetchupClient();
		}

		[Then(@"the value should be added")]
		public void ThenTheValueShouldBeAdded() {
			var result = ketchup.Get<string>("Hello");
			Assert.AreEqual<string>("World", result);
		}

		[When(@"Set is executed with a key and a value")]
		public void WhenSetIsExecutedWithAKeyAndAValue() {
			var result = ketchup.Set("Hello", "World");
		}

	}
}
