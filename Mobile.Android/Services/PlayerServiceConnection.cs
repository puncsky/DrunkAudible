// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Android.Content;
using Android.OS;
using System;

namespace DrunkAudible.Mobile.Android
{
    /// <summary>
    /// Connection to bind to the binder
    /// </summary>
    public class PlayerServiceConnection : Java.Lang.Object, IServiceConnection
    {
        PlayerPresenterFragment _fragment;
        PlayerServiceBinder _binder;

        public PlayerServiceConnection (PlayerPresenterFragment fragment)
        {
            if (fragment == null)
            {
                throw new ArgumentNullException ("fragment");
            }

            _fragment = fragment;
        }

        public void OnServiceConnected (ComponentName name, IBinder service)
        {
            _binder = (PlayerServiceBinder)service;

            _fragment.IsBound = _binder != null;
        }

        public void OnServiceDisconnected (ComponentName name)
        {
            _fragment.IsBound = false;
        }

        public PlayerServiceBinder Binder { get { return _binder; } }
    }
}

