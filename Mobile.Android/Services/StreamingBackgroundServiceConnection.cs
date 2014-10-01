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
        AudioPlayerFragment _fragment;
        StreamingBackgroundServiceBinder _binder;

        public StreamingBackgroundServiceConnection (AudioPlayerFragment fragment)
        {
            if (fragment == null)
            {
                throw new ArgumentNullException ("fragment");
            }

            _fragment = fragment;
        }

        public void OnServiceConnected (ComponentName name, IBinder service)
        {
            _binder = (StreamingBackgroundServiceBinder)service;

            if (_binder == null)
            {
                _fragment.IsBound = false;
                return;
            }

            _fragment.IsBound = true;

            if (_fragment.CurrentAlbum != null)
            {
                _binder.Service.CurrentAlbum = _fragment.CurrentAlbum;
            }
            else
            {
                _fragment.CurrentAlbum = _binder.Service.CurrentAlbum;
            }

            if (_fragment.CurrentEpisode != null)
            {
                _binder.Service.CurrentEpisode = _fragment.CurrentEpisode;
            }
            else
            {
                _fragment.CurrentEpisode = _binder.Service.CurrentEpisode;
            }
        }

        public void OnServiceDisconnected (ComponentName name)
        {
            _fragment.IsBound = false;
        }

        public StreamingBackgroundServiceBinder Binder { get { return _binder; } }
    }
}

