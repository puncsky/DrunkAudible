// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using DrunkAudible.Mobile;
using UIKit;
using Foundation;

namespace Mobile.iOS
{
    class iOSImageDownloader : ImageDownloader
    {
        public iOSImageDownloader ()
            : base (maxConcurrentDownloads: 2)
        {
        }

        protected override object LoadImage (System.IO.Stream stream)
        {
            return new UIImage (NSData.FromStream (stream));
        }
    }
}

