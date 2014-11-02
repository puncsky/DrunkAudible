// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using Android.App;
using Android.Runtime;
using DrunkAudible.Data;
using DrunkAudible.Data.Models;

namespace DrunkAudible.Mobile.Android
{
    // TODO Remove debuggable for the release version.
    [Application (Debuggable=true)]
    public class DrunkAudibleApplication : Application
    {
        Album _currentAlbum = Album.Empty;

        AudioEpisode _currentEpisode = AudioEpisode.Empty;

        DrunkAudibleMobileDatabase _database = new DrunkAudibleMobileDatabase ();

        // Must specify the ctor. Otherwise, System.NotSupportedException will be thrown.
        public DrunkAudibleApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            Self = this;
        }

        public Album CurrentAlbum { get { return _currentAlbum; } set { _currentAlbum = value; } }

        public AudioEpisode CurrentEpisode { get { return _currentEpisode; } set { _currentEpisode = value; } }

        public DrunkAudibleMobileDatabase Database { get { return _database; } set { value = _database; } }

        public static DrunkAudibleApplication Self { get; private set; }
    }
}

