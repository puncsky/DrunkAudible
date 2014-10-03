// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DrunkAudible.Data.Models;
using Android.Views;

namespace DrunkAudible.Mobile.Android
{
    public class PlayerPresenterFragment : Fragment
    {
        PlayerServiceConnection _connection;

        const int INTERVAL = 200; // milliseconds per update of UI.

        const String TIME_FORMAT = "{0:mm\\:ss}";

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

            _connection = new PlayerServiceConnection (this);
            _pauseIconString = Activity.ApplicationContext.GetString (Resource.String.ic_fa_pause);
            _playIconString = Activity.ApplicationContext.GetString (Resource.String.ic_fa_play);

            InitializeViews ();

            _uIUpdateTimer = new Timer (
                o => Activity.RunOnUiThread (UpdateFromPlayerService), // Must RunOnUiThread to update.
                null,
                Timeout.Infinite,
                Timeout.Infinite
            );
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

            if (!IsBound)
            {
                SendAudioCommand (PlayerService.ACTION_CONNECT);
            }
            UpdateProgress ((int) CurrentEpisode.Duration, (int) CurrentEpisode.CurrentTime);
            StartUpdatingFromPlayerService ();
        }

        public override void OnStop ()
        {
            base.OnStop ();

            if (IsBound)
            {
                Activity.UnbindService (_connection);
                IsBound = false;
            }
            StopUpdatingFromPlayerService ();
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
            get { return ExtraUtils.GetAlbum (Activity.Intent); }
            set
            {
                ExtraUtils.PutAlbum (Activity.Intent, value.Id);
            }
        }

        public AudioEpisode CurrentEpisode
        {
            get { return ExtraUtils.GetAudioEpisode (Activity.Intent, CurrentAlbum); }
            set
            {
                ExtraUtils.PutEpisode (Activity.Intent, value.Id);
            }
        }

        void InitializeViews ()
        {
            _seekBar = Activity.FindViewById<SeekBar> (Resource.Id.SeekBar);
            _spentTime = Activity.FindViewById<TextView> (Resource.Id.SpentTime);
            _leftTime = Activity.FindViewById<TextView> (Resource.Id.LeftTime);
            UpdateProgress ((int) CurrentEpisode.Duration, (int) CurrentEpisode.CurrentTime);
            _seekBar.StartTrackingTouch += (sender, e) => StopUpdatingFromPlayerService ();
            _seekBar.ProgressChanged += (sender, e) =>
            {
                // The player moves to the position which the user drags to.
                if (e.FromUser && IsBound)
                {
                    UpdateProgress (_seekBar.Max, _seekBar.Progress);
                }
            };
            _seekBar.StopTrackingTouch += (sender, e) =>
            {
                _connection.Binder.Service.CurrentPosition = _seekBar.Progress;
                StartUpdatingFromPlayerService ();
            };

            _playOrPauseButton = Activity.FindViewById<Button> (Resource.Id.PlayOrPauseButton);
            IconProvider.ConvertTextViewToIcon (Activity.Assets, _playOrPauseButton);
            _playOrPauseButton.Click += (sender, args) =>
            {
                if (!IsPlaying)
                {
                    SendAudioCommand (PlayerService.ACTION_PLAY);
                }
                else
                {
                    SendAudioCommand (PlayerService.ACTION_PAUSE);
                }
            };

            _title = Activity.FindViewById<TextView> (Resource.Id.PlayerTitle);
            _title.Text = CurrentEpisode.Title;
        }

        void SendAudioCommand (string action)
        {
            var intent = PlayerService.CreateIntent (Activity, action);

            if (!IsBound)
            {
                Activity.BindService (intent, _connection, Bind.AutoCreate);
            }

            Activity.StartService (intent);
        }

        void StartUpdatingFromPlayerService ()
        {
            _uIUpdateTimer.Change (0, INTERVAL);
        }

        void StopUpdatingFromPlayerService ()
        {
            _uIUpdateTimer.Change (Timeout.Infinite, Timeout.Infinite);
        }

        void UpdateFromPlayerService ()
        {
            if (IsPlaying)
            {
                UpdateProgress (
                    _connection.Binder.Service.Duration,
                    _connection.Binder.Service.CurrentPosition
                );

                _playOrPauseButton.Text = _pauseIconString;
                CurrentAlbum = _connection.Binder.Service.CurrentAlbum;
                CurrentEpisode = _connection.Binder.Service.CurrentEpisode;
                _title.Text = CurrentEpisode.Title;
            }
            else
            {
                _playOrPauseButton.Text = _playIconString;
            }
        }

        void UpdateProgress (int progressMax, int progress)
        {
            UpdateSeekBar (progressMax, progress);
            UpdateTimerTextViews (progressMax, progress);
            CurrentEpisode.CurrentTime = progress;
        }

        void UpdateSeekBar (int progressMax, int progress)
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

