// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Android.Content;
using Android.OS;

namespace DrunkAudible.Mobile.Android
{
    /// <summary>
    /// Connection to bind to the binder
    /// </summary>
    public class StreamingBackgroundServiceConnection : Java.Lang.Object, IServiceConnection
    {
        AudioPlayerActivity _activity;
        StreamingBackgroundServiceBinder _binder;

        public StreamingBackgroundServiceConnection (AudioPlayerActivity activity)
        {
            _activity = activity;
        }

        public void OnServiceConnected (ComponentName name, IBinder service)
        {
            _binder = (StreamingBackgroundServiceBinder)service;
            _activity.IsBound |= _binder != null;
        }

        public void OnServiceDisconnected (ComponentName name)
        {
            _activity.IsBound = false;
        }

        public StreamingBackgroundServiceBinder Binder { get { return _binder; } }
    }
}

