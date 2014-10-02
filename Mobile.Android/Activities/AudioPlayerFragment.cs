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

        const int INTERVAL = 200; // milliseconds per update of UI.

        const String TIME_FORMAT = "{0:mm\\:ss}";

        Album _currentAlbum = Album.Empty;
        AudioEpisode _currentEpisode = AudioEpisode.Empty;

        TextView _title;
        SeekBar _seekBar;
        TextView _spentTime;
        TextView _leftTime;
        Timer _uIUpdateTimer;

        Button _playOrPauseButton;
        String _playIconString;
        String _pauseIconString;

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
        }

        public static Intent CreateIntent(Context context, int albumID, int currentEpisodeID)
        {
            var intent = new Intent (context, typeof (MainActivity));
            ExtraUtils.PutAlbum (intent, albumID);
            ExtraUtils.PutEpisode(intent, currentEpisodeID);
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

        public bool IsPlaying
        {
            get
            {
                return IsBound && _connection.Binder.Service.IsPlaying;
            }
        }

        public Album CurrentAlbum
        {
            get { return _currentAlbum; }
            set
            {
                _currentAlbum = value;
                if (_currentAlbum == null)
                {
                    _currentAlbum = Album.Empty;
                }
            }
        }

        public AudioEpisode CurrentEpisode
        {
            get { return _currentEpisode; }
            set
            {
                _currentEpisode = value;
                if (_currentEpisode == null)
                {
                    _currentEpisode = AudioEpisode.Empty;
                }
                if (_title != null)
                {
                    _title.Text = _currentEpisode.Title;
                }
            }
        }

        void Initialize ()
        {
            _connection = new StreamingBackgroundServiceConnection (this);
            _pauseIconString = Activity.ApplicationContext.GetString (Resource.String.ic_fa_pause);
            _playIconString = Activity.ApplicationContext.GetString (Resource.String.ic_fa_play);

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
            CurrentAlbum = ExtraUtils.GetAlbum (Activity.Intent);
            CurrentEpisode = ExtraUtils.GetAudioEpisode (Activity.Intent, CurrentAlbum);
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

            _playOrPauseButton = Activity.FindViewById<Button> (Resource.Id.PlayOrPauseButton);
            IconProvider.ConvertTextViewToIcon (Activity.Assets, _playOrPauseButton);
            _playOrPauseButton.Click += (sender, args) =>
            {
                if (!IsPlaying)
                {
                    SendAudioCommand (StreamingBackgroundService.ACTION_PLAY);
                }
                else
                {
                    SendAudioCommand (StreamingBackgroundService.ACTION_PAUSE);
                }
            };

            _title = Activity.FindViewById<TextView> (Resource.Id.PlayerTitle);
            _title.Text = CurrentEpisode.Title;
        }

        void SendAudioCommand (string action)
        {
            var intent = StreamingBackgroundService.CreateIntent (Activity, action);
            if (StreamingBackgroundService.ACTION_PLAY)
            {
                ExtraUtils.PutAlbum (intent, CurrentAlbum.Id);
                ExtraUtils.PutEpisode (intent, CurrentEpisode.Id);
            }

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
            if (IsPlaying)
            {
                UpdateTimerViewsAndStates (
                    _connection.Binder.Service.Duration,
                    _connection.Binder.Service.CurrentPosition
                );

                _playOrPauseButton.Text = _pauseIconString;
            }
            else
            {
                _playOrPauseButton.Text = _playIconString;
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

