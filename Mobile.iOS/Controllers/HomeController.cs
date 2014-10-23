// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using Foundation;
using UIKit;
using DrunkAudible.Data.Models;
using System.IO;
using DrunkAudible.Data;

namespace Mobile.iOS
{
    public partial class HomeController : UIViewController
    {
        public HomeController (IntPtr handle)
            : base (handle)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            var albums = AppDelegate.Self.Database.Albums;
            _albumTableView.Source = new IconAndTitleItemTableViewSource (albums);
        }
    }
}

