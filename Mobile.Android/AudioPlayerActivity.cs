// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace DrunkAudible.Mobile.Android
{
    [Activity (
        Label = "BackgroundStreamingAudio",
        MainLauncher = true,
        Icon = "@drawable/ic_launcher",
        Theme = "@style/Theme"
    )]
    public class AudioPlayerActivity : Activity
    {
        StreamingBackgroundServiceConnection _connection;

        const int INTERVAL = 1000; // milliseconds per update of audio player seekbar.

        const String TIME_FORMAT = "{0:mm\\:ss}";

        SeekBar _seekBar;

        CancellationTokenSource _cancellationTokenSource;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            SetContentView (Resource.Layout.AudioPlayer);

            _connection = new StreamingBackgroundServiceConnection (this);

            var play = FindViewById<Button> (Resource.Id.playButton);
            var pause = FindViewById<Button> (Resource.Id.pauseButton);
            var stop = FindViewById<Button> (Resource.Id.stopButton);
            var spentTime = FindViewById<TextView> (Resource.Id.SpentTime);
            var leftTime = FindViewById<TextView> (Resource.Id.LeftTime);
            _seekBar = FindViewById<SeekBar> (Resource.Id.SeekBar);

            play.Click += (sender, args) => SendAudioCommand (StreamingBackgroundService.ACTION_PLAY);
            pause.Click += (sender, args) => SendAudioCommand (StreamingBackgroundService.ACTION_PAUSE);
            stop.Click += (sender, args) =>
            {
                SendAudioCommand (StreamingBackgroundService.ACTION_STOP);
                _seekBar.Progress = 0;
            };
            _seekBar.ProgressChanged += (sender, e) =>
            {
                spentTime.Text = String.Format (TIME_FORMAT, TimeSpan.FromSeconds (_seekBar.Progress));
                leftTime.Text = String.Format (TIME_FORMAT, TimeSpan.FromSeconds (_seekBar.Max - _seekBar.Progress));
                if (e.FromUser && IsBound)
                {
                    _connection.Binder.Service.CurrentPosition = _seekBar.Progress;
                }
            };

            SendAudioCommand (StreamingBackgroundService.ACTION_PLAY);
        }

        protected override void OnStart ()
        {
            base.OnStart ();

            StartUpdatingSeekBarProgress ();
        }

        protected override void OnStop ()
        {
            base.OnStop ();

            if (IsBound)
            {
                UnbindService (_connection);
                IsBound = false;
            }
            StopUpdatingSeekBarProgress ();
        }

        public bool IsBound { get; set; }

        void SendAudioCommand (string action)
        {
            var intent = new Intent (action);
            if (!IsBound)
            {
                BindService (intent, _connection, Bind.AutoCreate);
            }
            StartService (intent);
        }

        void StartUpdatingSeekBarProgress ()
        {
            _cancellationTokenSource = new CancellationTokenSource ();
            PeriodicTaskFactory.Start (
                UpdateSeekBarProgress,
                INTERVAL,
                cancelToken: _cancellationTokenSource.Token);
        }

        void StopUpdatingSeekBarProgress ()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel ();
                _cancellationTokenSource = null;
            }
        }

        void UpdateSeekBarProgress ()
        {
            if (IsBound && _connection.Binder.Service.IsPlaying)
            {
                _seekBar.Max = _connection.Binder.Service.Duration;
                _seekBar.Progress = _connection.Binder.Service.CurrentPosition;
            }
        }
    }
}

