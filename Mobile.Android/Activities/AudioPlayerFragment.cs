// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DrunkAudible.Data.Models;
using Android.Views;

namespace DrunkAudible.Mobile.Android
{
    public class AudioPlayerFragment : Fragment
    {
        StreamingBackgroundServiceConnection _connection;

        const int INTERVAL = 100; // milliseconds per update of audio player seekbar.

        const String TIME_FORMAT = "{0:mm\\:ss}";

        public const String ALBUM_ID_INTENT_EXTRA = "AlbumID";
        public const String EPISODE_ID_INTENT_EXTRA = "EpisodeID";

        Album _currentAlbum = new Album ();
        AudioEpisode _currentEpisode = new AudioEpisode ();

        SeekBar _seekBar;
        TextView _spentTime;
        TextView _leftTime;
        Timer _uIUpdateTimer;

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState)
        {
            base.OnCreateView (inflater, container, savedInstanceState);

            return inflater.Inflate (Resource.Layout.AudioPlayer, container, false);
        }

        public override void OnActivityCreated (Bundle savedInstanceState)
        {
            base.OnActivityCreated (savedInstanceState);

            Initialize ();

            SendAudioCommand (StreamingBackgroundService.ACTION_CONNECT);
        }

        public static Intent CreateIntent(Context context, int albumID, int currentEpisodeID)
        {
            var intent = new Intent (context, typeof (MainActivity));
            intent.PutExtra (ALBUM_ID_INTENT_EXTRA, albumID);
            intent.PutExtra (EPISODE_ID_INTENT_EXTRA, currentEpisodeID);
            return intent;
        }

        public override void OnStart ()
        {
            base.OnStart ();

            SendAudioCommand (StreamingBackgroundService.ACTION_CONNECT);
            UpdateTimerViewsAndStates ((int) CurrentEpisode.Duration, (int) CurrentEpisode.CurrentTime);
            StartUpdateTimerViewsAndStatesFromPlayerService ();
        }

        public override void OnStop ()
        {
            base.OnStop ();

            if (IsBound)
            {
                Activity.UnbindService (_connection);
                IsBound = false;
            }
            StopUpdateTimerViewsAndStatesFromPlayerService ();
        }

        public override void OnDestroyView ()
        {
            base.OnDestroyView ();

            _uIUpdateTimer.Dispose ();
        }

        public bool IsBound { get; set; }

        public Album CurrentAlbum { get { return _currentAlbum; } set { _currentAlbum = value; } }

        public AudioEpisode CurrentEpisode { get { return _currentEpisode; } set { _currentEpisode = value; } }

        void Initialize ()
        {
            _connection = new StreamingBackgroundServiceConnection (this);

            InitializeExtrasFromIntent ();
            InitializeViews ();

            _uIUpdateTimer = new Timer (
                o => Activity.RunOnUiThread (UpdateTimerViewsAndStatesFromPlayerService), // Must RunOnUiThread to update.
                null,
                Timeout.Infinite,
                Timeout.Infinite
            );
        }

        void InitializeExtrasFromIntent ()
        {
            if (Activity.Intent.HasExtra (ALBUM_ID_INTENT_EXTRA))
            {
                CurrentAlbum = DatabaseSingleton
                    .Orm
                    .Albums
                    .FirstOrDefault (a => a.Id == Activity.Intent.GetIntExtra (ALBUM_ID_INTENT_EXTRA, -1));
            }

            if (Activity.Intent.HasExtra (EPISODE_ID_INTENT_EXTRA))
            {
                CurrentEpisode = CurrentAlbum
                    .Episodes
                    .FirstOrDefault (e => e.Id == Activity.Intent.GetIntExtra (EPISODE_ID_INTENT_EXTRA, -1));
            }
        }

        void InitializeViews ()
        {
            _seekBar = Activity.FindViewById<SeekBar> (Resource.Id.SeekBar);
            _spentTime = Activity.FindViewById<TextView> (Resource.Id.SpentTime);
            _leftTime = Activity.FindViewById<TextView> (Resource.Id.LeftTime);
            UpdateTimerViewsAndStates ((int) CurrentEpisode.Duration, (int) CurrentEpisode.CurrentTime);
            _seekBar.StartTrackingTouch += (sender, e) => StopUpdateTimerViewsAndStatesFromPlayerService ();
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
                StartUpdateTimerViewsAndStatesFromPlayerService ();
            };

            var play = Activity.FindViewById<Button> (Resource.Id.playButton);
            play.Click += (sender, args) => SendAudioCommand (StreamingBackgroundService.ACTION_PLAY);

            var pause = Activity.FindViewById<Button> (Resource.Id.pauseButton);
            pause.Click += (sender, args) => SendAudioCommand (StreamingBackgroundService.ACTION_PAUSE);

            var stop = Activity.FindViewById<Button> (Resource.Id.stopButton);
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
                Activity.BindService (intent, _connection, Bind.AutoCreate);
            }

            Activity.StartService (intent);
        }

        void StartUpdateTimerViewsAndStatesFromPlayerService ()
        {
            _uIUpdateTimer.Change (0, INTERVAL);
        }

        void StopUpdateTimerViewsAndStatesFromPlayerService ()
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

