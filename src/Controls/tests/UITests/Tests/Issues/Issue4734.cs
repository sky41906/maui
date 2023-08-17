using Microsoft.Maui.Appium;
using NUnit.Framework;

namespace Microsoft.Maui.AppiumTests.Issues
{
	public class Issue4734 : _IssuesUITest
	{
		public Issue4734(TestDevice device)
		: base(device)
		{ }

		public override string Issue => "Gestures in Label Spans not working";

		[Test]
		public void Issue4734Test()
		{
			if (Device == TestDevice.Windows)
			{
				Assert.Ignore("This test is failing, likely due to product issue");
			}
			else
			{
				App.WaitForElement("WaitForStubControl");
				App.Tap("TargetSpan");
				var textAfterTap = App.Query("TapResultLabel").First().Text;
				Assert.False(string.IsNullOrEmpty(textAfterTap));
			}
		}
	}
}