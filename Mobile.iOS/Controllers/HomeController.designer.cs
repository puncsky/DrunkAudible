// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace Mobile.iOS
{
	[Register ("HomeController")]
	partial class HomeController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView _albumTableView { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (_albumTableView != null) {
				_albumTableView.Dispose ();
				_albumTableView = null;
			}
		}
	}
}
