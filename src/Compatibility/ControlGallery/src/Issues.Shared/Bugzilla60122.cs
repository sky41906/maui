﻿using System;
using Microsoft.Maui.Controls.CustomAttributes;
using Microsoft.Maui.Controls.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Microsoft.Maui.Controls.ControlGallery.Issues
{
#if UITEST
	[NUnit.Framework.Category(Compatibility.UITests.UITestCategories.Bugzilla)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Bugzilla, 60122, "LongClick on image not working", PlatformAffected.Android)]
	public class Bugzilla60122 : TestContentPage
	{
		const string ImageId = "60122Image";
		const string Success = "Success";

		protected override void Init()
		{
			var customImage = new _60122Image
			{
				AutomationId = ImageId,
				Source = "coffee.png"
			};

			var instructions = new Label
			{
				Text = $"Long press the image below; the label below it should change to read {Success}"
			};

			var result = new Label { Text = "Testing..." };

			customImage.LongPress += (sender, args) => { result.Text = Success; };

			Content = new StackLayout
			{
				Children = { instructions, customImage, result }
			};
		}

		public class _60122Image : Image
		{
			public event EventHandler LongPress;

			public void HandleLongPress(object sender, EventArgs e)
			{
				LongPress?.Invoke(this, new EventArgs());
			}
		}

#if UITEST && !WINDOWS

		// This test won't work on Windows right now because we can only test desktop, so touch events
		// (like LongPress) don't really work. The test should work manually on a touch screen, though.

[Microsoft.Maui.Controls.Compatibility.UITests.FailsOnMauiAndroid]
		[Test]
		public void LongClickFiresOnCustomImageRenderer()
		{
			RunningApp.WaitForElement(ImageId);
			RunningApp.TouchAndHold(ImageId);
			RunningApp.WaitForElement(Success);
		}
#endif
	}
}