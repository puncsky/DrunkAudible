// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Foundation;
using UIKit;
using DrunkAudible.Data;
using DrunkAudible.Data.Models;
using System.IO;

namespace Mobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        Album _currentAlbum = Album.Empty;

        AudioEpisode _currentEpisode = AudioEpisode.Empty;

        readonly DrunkAudibleMobileDatabase _database = new DrunkAudibleMobileDatabase ();

        public override void FinishedLaunching (UIApplication application)
        {
            AppDelegate.Self = this;
        }

        public override bool WillFinishLaunching (UIApplication application, NSDictionary launchOptions)
        {
            var dbResource = NSBundle.MainBundle.PathForResource ("DrunkAudible.Mobile.SQLite", "db3");
            // TODO install only once for release version.
//            if (!File.Exists (DrunkAudibleMobileDatabase.DatabasePath))
//            {
            File.Copy (dbResource, DrunkAudibleMobileDatabase.DatabasePath, overwrite: true);
//            }

            return true;
        }

        public override UIWindow Window
        {
            get;
            set;
        }

        public static AppDelegate Self
        {
            get;
            set;
        }

        public Album CurrentAlbum { get { return _currentAlbum; } set { _currentAlbum = value; } }

        public AudioEpisode CurrentEpisode { get { return _currentEpisode; } set { _currentEpisode = value; } }

        public DrunkAudibleMobileDatabase Database { get { return _database; } set { value = _database; } }
    }
}

