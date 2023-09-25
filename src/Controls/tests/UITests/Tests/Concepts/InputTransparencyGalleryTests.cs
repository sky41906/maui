﻿using Maui.Controls.Sample;
using Microsoft.Maui.Appium;
using NUnit.Framework;

namespace Microsoft.Maui.AppiumTests
{
	public class InputTransparencyGalleryTests : CoreGalleryBasePageTest
	{
		const string ButtonGallery = "* marked:'Input Transparency Gallery'";

		public InputTransparencyGalleryTests(TestDevice device)
			: base(device)
		{
		}

		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(ButtonGallery);
		}

		[Test]
		public void Simple([Values] Test.InputTransparency test) =>
			RunTest(test.ToString(), !test.ToString().EndsWith("InputBlocked"));

		[Test]
		[Combinatorial]
		public void Matrix([Values] bool rootTrans, [Values] bool rootCascade, [Values] bool nestedTrans, [Values] bool nestedCascade, [Values] bool trans)
		{
			var (clickable, passthru) = Test.InputTransparencyMatrix.States[(rootTrans, rootCascade, nestedTrans, nestedCascade, trans)];
			var key = Test.InputTransparencyMatrix.GetKey(rootTrans, rootCascade, nestedTrans, nestedCascade, trans, clickable, passthru);

			RunTest(key, clickable || passthru);
		}

		static void RunTest(string test, bool updatable)
		{
			var remote = new EventViewContainerRemote(UITestContext, test);
			remote.GoTo(test.ToString());

			var textBeforeClick = remote.GetEventLabel().Text;
			Assert.AreEqual($"Event: {test} (none)", textBeforeClick);

			remote.TapView();

			var textAfterClick = remote.GetEventLabel().Text;

			if (updatable)
			{
				// if the button is clickable or taps pass through to the base button
				Assert.AreEqual($"Event: {test} (SUCCESS 1)", textAfterClick);
			}
			else
			{
				// sometimes nothing can happen, so try test that
				Task.Delay(500).Wait(); // just make sure that nothing happened

				textAfterClick = remote.GetEventLabel().Text;
				Assert.AreEqual($"Event: {test} (none)", textBeforeClick);
			}
		}
	}
}
