// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DrunkAudible.Data.Models;

namespace DrunkAudible.Mobile.Android
{
    [Activity (Label = "AudioPlayerActivity")]
    public class AudioPlayerActivity : Activity
    {
        StreamingBackgroundServiceConnection _connection;

        const int INTERVAL = 100; // milliseconds per update of audio player seekbar.

        const String TIME_FORMAT = "{0:mm\\:ss}";

        const String ALBUM_ID_INTENT_EXTRA = "AlbumID";
        const String EPISODE_ID_INTENT_EXTRA = "EpisodeID";

        SeekBar _seekBar;
        TextView _spentTime;
        TextView _leftTime;
        Timer _uIUpdateTimer;

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            Initialize ();

            SendAudioCommand (StreamingBackgroundService.ACTION_CONNECT);
        }

        public static Intent CreateIntent(Context context, int albumID, int currentEpisodeID)
        {
            var intent = new Intent (context, typeof (AudioPlayerActivity));
            intent.PutExtra (ALBUM_ID_INTENT_EXTRA, albumID);
            intent.PutExtra (EPISODE_ID_INTENT_EXTRA, currentEpisodeID);
            return intent;
        }

        protected override void OnStart ()
        {
            base.OnStart ();

            SendAudioCommand (StreamingBackgroundService.ACTION_CONNECT);
            StartUpdatingTimerViewsAndStates ();
        }

        protected override void OnStop ()
        {
            base.OnStop ();

            if (IsBound)
            {
                UnbindService (_connection);
                IsBound = false;
            }
            StopUpdatingTimerViewsAndStates ();
        }

        public bool IsBound { get; set; }

        public Album CurrentAlbum { get; set; }

        public AudioEpisode CurrentEpisode { get; set; }

        void Initialize ()
        {
            InitializeExtrasFromIntent ();
            InitializeViews ();

            _uIUpdateTimer = new Timer (
                o => RunOnUiThread (UpdateTimerViewsAndStatesFromPlayerService), // Must RunOnUiThread to update.
                null,
                Timeout.Infinite,
                Timeout.Infinite
            );
        }

        void InitializeExtrasFromIntent ()
        {
            if (Intent.HasExtra (ALBUM_ID_INTENT_EXTRA))
            {
                CurrentAlbum = DatabaseSingleton
                    .Orm
                    .Albums
                    .FirstOrDefault (a => a.Id == Intent.GetIntExtra (ALBUM_ID_INTENT_EXTRA, -1));
            }
            if (Intent.HasExtra (EPISODE_ID_INTENT_EXTRA))
            {
                CurrentEpisode = CurrentAlbum
                    .Episodes
                    .FirstOrDefault (e => e.Id == Intent.GetIntExtra (EPISODE_ID_INTENT_EXTRA, -1));
            }
        }

        void InitializeViews ()
        {
            SetContentView (Resource.Layout.AudioPlayer);

            _seekBar = FindViewById<SeekBar> (Resource.Id.SeekBar);
            _spentTime = FindViewById<TextView> (Resource.Id.SpentTime);
            _leftTime = FindViewById<TextView> (Resource.Id.LeftTime);
            UpdateTimerViewsAndStates ((int) CurrentEpisode.Duration, (int) CurrentEpisode.CurrentTime);
            _seekBar.StartTrackingTouch += (sender, e) => StopUpdatingTimerViewsAndStates ();
            _seekBar.ProgressChanged += (sender, e) =>
            {
                // The player moves to the position which the user drags to.
                if (e.FromUser && IsBound)
                {
                    UpdateTimerViewsAndStates (_seekBar.Max, _seekBar.Progress);
                }
            };
            _seekBar.StopTrackingTouch += (sender, e) =>
            {
                _connection.Binder.Service.CurrentPosition = _seekBar.Progress;
                StartUpdatingTimerViewsAndStates ();
            };

            _connection = new StreamingBackgroundServiceConnection (this);

            var play = FindViewById<Button> (Resource.Id.playButton);
            play.Click += (sender, args) => SendAudioCommand (StreamingBackgroundService.ACTION_PLAY);

            var pause = FindViewById<Button> (Resource.Id.pauseButton);
            pause.Click += (sender, args) => SendAudioCommand (StreamingBackgroundService.ACTION_PAUSE);

            var stop = FindViewById<Button> (Resource.Id.stopButton);
            stop.Click += (sender, args) =>
            {
                UpdateTimerViewsAndStates (_seekBar.Max, 0);
                SendAudioCommand (StreamingBackgroundService.ACTION_PAUSE);
            };
        }

        void SendAudioCommand (string action)
        {
            var intent = StreamingBackgroundService.CreateIntent (action);

            if (!IsBound)
            {
                BindService (intent, _connection, Bind.AutoCreate);
            }

            StartService (intent);
        }

        void StartUpdatingTimerViewsAndStates ()
        {
            _uIUpdateTimer.Change (0, INTERVAL);
        }

        void StopUpdatingTimerViewsAndStates ()
        {
            _uIUpdateTimer.Change (Timeout.Infinite, Timeout.Infinite);
        }

        void UpdateTimerViewsAndStatesFromPlayerService ()
        {
            if (IsBound && _connection.Binder.Service.IsPlaying)
            {
                UpdateTimerViewsAndStates (
                    _connection.Binder.Service.Duration,
                    _connection.Binder.Service.CurrentPosition
                );
            }
        }

        void UpdateTimerViewsAndStates (int progressMax, int progress)
        {
            UpdateSeekBarProgress (progressMax, progress);
            UpdateTimerTextViews (progressMax, progress);
            CurrentEpisode.CurrentTime = progress;
        }

        void UpdateSeekBarProgress (int progressMax, int progress)
        {
            _seekBar.Max = progressMax;
            _seekBar.Progress = progress;
        }

        void UpdateTimerTextViews (int progressMax, int progress)
        {
            _spentTime.Text = String.Format (TIME_FORMAT, TimeSpan.FromSeconds (progress));
            _leftTime.Text = String.Format (
                TIME_FORMAT,
                TimeSpan.FromSeconds (progressMax - progress)
            );
        }
    }
}

