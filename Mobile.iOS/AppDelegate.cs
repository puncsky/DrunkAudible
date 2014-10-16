// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Foundation;
using UIKit;
using DrunkAudible.Data.Models;

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

        // This method is invoked when the application is about to move from active to inactive state.
        // OpenGL applications should use this method to pause.
        public override void OnResignActivation (UIApplication application)
        {
        }

        // This method should be used to release shared resources and it should store the application state.
        // If your application supports background exection this method is called instead of WillTerminate
        // when the user quits.
        public override void DidEnterBackground (UIApplication application)
        {
        }
        
        // This method is called as part of the transiton from background to active state.
        public override void WillEnterForeground (UIApplication application)
        {
        }

        // This method is called when the application is about to terminate. Save data, if needed.
        public override void WillTerminate (UIApplication application)
        {
        }

        public override void FinishedLaunching (UIApplication application)
        {
            AppDelegate.Self = this;
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
    }
}

