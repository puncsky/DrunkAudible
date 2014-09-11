// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Net.Wifi;
using Android.OS;
using Android.Util;
using Net = Android.Net;

namespace Mobile.Android
{
    [Service]
    [IntentFilter (new[] { ACTION_PLAY, ACTION_PAUSE, ACTION_STOP })]
    public class StreamingBackgroundService : Service, AudioManager.IOnAudioFocusChangeListener
    {
        // Actions
        public const string ACTION_PLAY = "com.puncsky.drunkaudible.mobile.android.action.PLAY";
        public const string ACTION_PAUSE = "com.puncsky.drunkaudible.mobile.android.action.PAUSE";
        public const string ACTION_STOP = "com.puncsky.drunkaudible.mobile.android.STOP";

        const String DEBUG_TAG = "StreamingBackgroundService";

        const string MP3 = "/data/data/warning.mp3";

        const int MILLISECONDS_PER_SECOND = 1000;

        MediaPlayer _player;
        AudioManager _audioManager;
        WifiManager _wifiManager;
        WifiManager.WifiLock _wifiLock;
        bool _paused;

        const int NotificationId = 1;

        /// <summary>
        /// On create simply detect some of our managers
        /// </summary>
        public override void OnCreate ()
        {
            base.OnCreate ();

            // Find our audio and notificaton managers
            _audioManager = (AudioManager)GetSystemService (AudioService);
            _wifiManager = (WifiManager)GetSystemService (WifiService);
        }

        public override IBinder OnBind (Intent intent)
        {
            return new StreamingBackgroundServiceBinder (this);
        }

        public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
        {
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
                        IntializePlayer ();

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
                return _player.IsPlaying;
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
            _player.Completion += (sender, args) => Stop ();

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
                Java.IO.File file = new Java.IO.File (MP3);
                Java.IO.FileInputStream fis = new Java.IO.FileInputStream (file);

                // TODO reorganize it to play selected audio.
                // await player.SetDataSourceAsync(ApplicationContext, Net.Uri.Parse(Mp3
                await _player.SetDataSourceAsync (fis.FD);

                var focusResult = _audioManager.RequestAudioFocus (this, Stream.Music, AudioFocus.Gain);
                if (focusResult != AudioFocusRequest.Granted)
                {
                    Log.Debug (DEBUG_TAG, "Could not get audio focus");
                }

                _player.PrepareAsync ();

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
            var pendingIntent = PendingIntent.GetActivity (
                ApplicationContext,
                0,
                new Intent (ApplicationContext, typeof(AudioPlayerActivity)),
                PendingIntentFlags.UpdateCurrent);

            var notification = new Notification
            {
                TickerText = new Java.Lang.String ("Song started!"),
                Icon = Resource.Drawable.ic_stat_av_play_over_video
            };
            notification.Flags |= NotificationFlags.OngoingEvent;
            notification.SetLatestEventInfo (
                ApplicationContext,
                "Xamarin Streaming",
                "Playing music!",
                pendingIntent
            );

            StartForeground (NotificationId, notification);
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
    }
}

