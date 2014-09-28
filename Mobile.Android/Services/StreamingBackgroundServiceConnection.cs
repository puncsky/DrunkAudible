// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Android.Content;
using Android.OS;
using System;

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
            if (activity == null)
            {
                throw new ArgumentNullException ("activity");
            }

            _activity = activity;
        }

        public void OnServiceConnected (ComponentName name, IBinder service)
        {
            _binder = (StreamingBackgroundServiceBinder)service;

            if (_binder == null)
            {
                _activity.IsBound = false;
                return;
            }

            _activity.IsBound = true;

            if (_activity.CurrentAlbum != null)
            {
                _binder.Service.CurrentAlbum = _activity.CurrentAlbum;
            }
            else
            {
                _activity.CurrentAlbum = _binder.Service.CurrentAlbum;
            }

            if (_activity.CurrentEpisode != null)
            {
                _binder.Service.CurrentEpisode = _activity.CurrentEpisode;
            }
            else
            {
                _activity.CurrentEpisode = _binder.Service.CurrentEpisode;
            }
        }

        public void OnServiceDisconnected (ComponentName name)
        {
            _activity.IsBound = false;
        }

        public StreamingBackgroundServiceBinder Binder { get { return _binder; } }
    }
}

