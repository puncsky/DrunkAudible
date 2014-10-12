// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using UIKit;

namespace Mobile.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main (string [] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main (args, null, "AppDelegate");
        }
    }
}
