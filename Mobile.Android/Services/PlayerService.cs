﻿// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Net.Wifi;
using Android.OS;
using Android.Util;
using DrunkAudible.Data.Models;
using Net = Android.Net;
using System.Threading;

namespace DrunkAudible.Mobile.Android
{
    [Service]
    [IntentFilter (new[] { ACTION_CONNECT, ACTION_PLAY, ACTION_PAUSE, ACTION_STOP })]
    public class PlayerService : Service, AudioManager.IOnAudioFocusChangeListener
    {
        // Actions
        public const string ACTION_CONNECT = "com.puncsky.drunkaudible.mobile.android.CONNECT";
        public const string ACTION_PLAY = "com.puncsky.drunkaudible.mobile.android.action.PLAY";
        public const string ACTION_PAUSE = "com.puncsky.drunkaudible.mobile.android.action.PAUSE";
        public const string ACTION_STOP = "com.puncsky.drunkaudible.mobile.android.STOP";

        const String DEBUG_TAG = "PlayerService";

        const double COMPARISON_EPSILON = 0.001;

        const int MILLISECONDS_PER_SECOND = 1000;
        const int EPISODE_CURRENT_TIME_UPDATE_INTERVAL = 10000; // in Milliseconds

        MediaPlayer _player;
        AudioManager _audioManager;
        WifiManager _wifiManager;
        WifiManager.WifiLock _wifiLock;
        bool _paused;
        Timer _episodeProgressUpdateTimer;

        const int NotificationId = 1;

        public override void OnCreate ()
        {
            base.OnCreate ();

            _episodeProgressUpdateTimer = new Timer (
                o => UpdateEpisodeCurrentTimeIfPlaying(),
                null,
                0,
                EPISODE_CURRENT_TIME_UPDATE_INTERVAL
            );

            // Find our audio and notificaton managers
            _audioManager = (AudioManager)GetSystemService (AudioService);
            _wifiManager = (WifiManager)GetSystemService (WifiService);
        }

        public static Intent CreateIntent(Context context, String action)
        {
            var intent = new Intent (context, typeof (PlayerService));
            intent.SetAction (action);
            return intent;
        }

        public static Intent CreateIntent(Context context, String action, int albumID, int episodeID)
        {
            var intent = CreateIntent(context, action);
            ExtraUtils.PutAlbum (intent, albumID);
            ExtraUtils.PutEpisode(intent, episodeID);
            return intent;
        }

        public override IBinder OnBind (Intent intent)
        {
            return new PlayerServiceBinder (this);
        }

        public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
        {
            if (intent == null)
            {
                // Re-launch the app after running with stop status in the background for a long time.
                // At this time, intent is null and throwing exceptions will break the app.
                return StartCommandResult.Sticky;
            }

            var extraAlbum = ExtraUtils.GetAlbum (DrunkAudibleApplication.Self.Database, intent);
            if (!Album.IsNullOrEmpty (extraAlbum))
            {
                CurrentAlbum = extraAlbum;
            }
            var extraEpisode = ExtraUtils.GetAudioEpisode (intent, CurrentAlbum);
            if (!AudioEpisode.IsNullOrEmpty (extraEpisode))
            {
                CurrentEpisode = extraEpisode;
            }

            switch (intent.Action)
            {
                case ACTION_PLAY:
                    Play ();
                    break;
                case ACTION_STOP:
                    Stop ();
                    break;
                case ACTION_PAUSE:
                    Pause ();
                    break;
            }

            //Set sticky as we are a long running operation
            return StartCommandResult.Sticky;
        }

        /// <summary>
        /// Properly cleanup of your player by releasing resources
        /// </summary>
        public override void OnDestroy ()
        {
            base.OnDestroy ();

            if (_player != null)
            {
                _player.Release ();
                _player = null;
            }

            if (_episodeProgressUpdateTimer != null)
            {
                _episodeProgressUpdateTimer.Dispose ();
            }
        }

        /// <summary>
        /// For a good user experience we should account for when audio focus has changed.
        /// There is only 1 audio output there may be several media services trying to use it so
        /// we should act correctly based on this.  "duck" to be quiet and when we gain go full.
        /// All applications are encouraged to follow this, but are not enforced.
        /// </summary>
        public void OnAudioFocusChange (AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain:
                    if (_player == null)
                    {
                        IntializePlayer ();
                    }

                    if (!_player.IsPlaying)
                    {
                        _player.Start ();
                        _paused = false;
                    }

                    _player.SetVolume (1.0f, 1.0f);//Turn it up!
                    break;
                case AudioFocus.Loss:
                    //We have lost focus stop!
                    Stop ();
                    break;
                case AudioFocus.LossTransient:
                    //We have lost focus for a short time, but likely to resume so pause
                    Pause ();
                    break;
                case AudioFocus.LossTransientCanDuck:
                    //We have lost focus but should till play at a muted 10% volume
                    if (_player.IsPlaying)
                        _player.SetVolume (.1f, .1f);//turn it down!
                    break;
            }
        }

        /// <summary>
        /// Gets the audio duration in seconds.
        /// </summary>
        /// <value>The duration in seconds.</value>
        public int Duration
        {
            get
            {
                return _player.Duration / MILLISECONDS_PER_SECOND;
            }
        }

        /// <summary>
        /// Get current position in seconds.
        /// </summary>
        /// <value>The current position in seconds.</value>
        public int CurrentPosition
        {
            get
            {
                return _player.CurrentPosition / MILLISECONDS_PER_SECOND;
            }
            set
            {
                _player.SeekTo (value * MILLISECONDS_PER_SECOND);
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _player != null && _player.IsPlaying;
            }
        }

        public Album CurrentAlbum
        {
            get { return DrunkAudibleApplication.Self.CurrentAlbum; }
            set { DrunkAudibleApplication.Self.CurrentAlbum = value; }
        }

        public AudioEpisode CurrentEpisode
        {
            get
            {
                return DrunkAudibleApplication.Self.CurrentEpisode; 
            }
            set
            {
                var isValidChange =
                    !AudioEpisode.IsNullOrEmpty (value) &&
                    value != DrunkAudibleApplication.Self.CurrentEpisode;
                if (isValidChange)
                {
                    Stop ();
                    DrunkAudibleApplication.Self.CurrentEpisode = value;
                    Play ();
                }
            }
        }

        AudioEpisode NextEpisode
        {
            get
            {
                var currentIndex =  CurrentAlbum.Episodes.IndexOf(CurrentEpisode);
                var nextIndex = currentIndex + 1;
                if (nextIndex < CurrentAlbum.Episodes.Count)
                {
                    var nextEpisode = CurrentAlbum.Episodes [nextIndex];
                    if (AudioDownloader.HasLocalFile (nextEpisode.RemoteUrl, nextEpisode.FileSize))
                    {
                        return nextEpisode;
                    }
                }

                return null;
            }
        }

        void IntializePlayer ()
        {
            _player = new MediaPlayer ();

            // Tell our player to sream music
            _player.SetAudioStreamType (Stream.Music);

            // Wake mode will be partial to keep the CPU still running under lock screen
            _player.SetWakeMode (ApplicationContext, WakeLockFlags.Partial);

            // When we have prepared the song start playback
            _player.Prepared += (sender, args) => _player.Start ();

            // When we have reached the end of the song stop ourselves, however you could signal next track here.
            _player.Completion += (sender, args) =>
            {
                Stop ();
                CurrentEpisode.CurrentTime = CurrentEpisode.Duration;
                DrunkAudibleApplication.Self.Database.InsertOrReplace (CurrentEpisode);
                if (NextEpisode != null)
                {
                    NextEpisode.CurrentTime = 0;
                    CurrentEpisode = NextEpisode;
                }
            };

            _player.Error += (sender, args) => 
            {
                // playback error
                Log.Debug (DEBUG_TAG, "Error in playback resetting: " + args.What);
                Stop ();//this will clean up and reset properly.
            };
        }

        async void Play ()
        {
            if (_paused && _player != null)
            {
                _paused = false;
                //We are simply paused so just start again
                _player.Start ();
                StartForeground ();
                return;
            }

            if (_player == null)
            {
                IntializePlayer ();
            }

            if (_player.IsPlaying)
            {
                return;
            }

            try
            {
                if (CurrentEpisode == null ||
                    Math.Abs (CurrentEpisode.Duration - CurrentEpisode.CurrentTime) < COMPARISON_EPSILON ||
                    !AudioDownloader.HasLocalFile (CurrentEpisode.RemoteUrl, CurrentEpisode.FileSize))
                {
                    return;
                }
                var file = new Java.IO.File (AudioDownloader.GetFilePath(CurrentEpisode.RemoteUrl));
                var fis = new Java.IO.FileInputStream (file);

                // TODO reorganize it to play selected audio.
                // await player.SetDataSourceAsync(ApplicationContext, Net.Uri.Parse(Mp3
                await _player.SetDataSourceAsync (fis.FD);

                var focusResult = _audioManager.RequestAudioFocus (this, Stream.Music, AudioFocus.Gain);
                if (focusResult != AudioFocusRequest.Granted)
                {
                    Log.Debug (DEBUG_TAG, "Could not get audio focus");
                }

                _player.Prepare();
                CurrentPosition = (int) CurrentEpisode.CurrentTime;

                AquireWifiLock ();
                StartForeground ();
            }
            catch (Exception ex)
            {
                Log.Debug (DEBUG_TAG, "Unable to start playback: " + ex);
            }
        }

        /// <summary>
        /// When we start on the foreground we will present a notification to the user
        /// When they press the notification it will take them to the main page so they can control the music
        /// </summary>
        void StartForeground ()
        {
            var intent = new Intent (ApplicationContext, typeof (MainActivity));
            intent.SetAction (Intent.ActionMain);
            intent.AddCategory (Intent.CategoryLauncher);
            ExtraUtils.PutSelectedTab (intent, (int) MainActivity.TabTitle.Player);

            var pendingIntent = PendingIntent.GetActivity (
                ApplicationContext,
                0,
                intent,
                PendingIntentFlags.UpdateCurrent
            );

            var notificationBuilder = new Notification.Builder (ApplicationContext);
            notificationBuilder.SetContentTitle (CurrentEpisode.Title);
            notificationBuilder.SetContentText (
                String.Format (
                    ApplicationContext.GetString (Resource.String.NotificationContentText),
                    ApplicationContext.GetString (Resource.String.ApplicationName)
                )
            );
            notificationBuilder.SetSmallIcon (Resource.Drawable.ic_stat_av_play_over_video);
            notificationBuilder.SetTicker (
                String.Format (
                    Application.GetString (Resource.String.NoticifationTicker),
                    CurrentEpisode.Title
                )
            );
            notificationBuilder.SetOngoing (true);
            notificationBuilder.SetContentIntent (pendingIntent);

            StartForeground (NotificationId, notificationBuilder.Build());
        }

        void Pause ()
        {
            if (_player == null)
            {
                return;
            }

            if (_player.IsPlaying)
            {
                _player.Pause ();
            }

            StopForeground (true);
            _paused = true;
        }

        void Stop ()
        {
            if (_player == null)
            {
                return;
            }

            if (_player.IsPlaying)
            {
                _player.Stop ();
            }

            _player.Reset ();
            _paused = false;
            StopForeground (true);
            ReleaseWifiLock ();
        }

        /// <summary>
        /// Lock the wifi so we can still stream under lock screen
        /// </summary>
        void AquireWifiLock ()
        {
            if (_wifiLock == null)
            {
                _wifiLock = _wifiManager.CreateWifiLock (Net.WifiMode.Full, "xamarin_wifi_lock");
            } 
            _wifiLock.Acquire ();
        }

        /// <summary>
        /// This will release the wifi lock if it is no longer needed
        /// </summary>
        void ReleaseWifiLock ()
        {
            if (_wifiLock == null)
            {
                return;
            }

            _wifiLock.Release ();
            _wifiLock = null;
        }

        void UpdateEpisodeCurrentTimeIfPlaying ()
        {
            if (IsPlaying)
            {
                CurrentEpisode.CurrentTime = CurrentPosition;
                DrunkAudibleApplication.Self.Database.InsertOrReplace (CurrentEpisode);
            }
        }
    }
}

