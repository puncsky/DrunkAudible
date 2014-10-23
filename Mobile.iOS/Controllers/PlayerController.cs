// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using AVFoundation;
using Foundation;
using UIKit;

namespace Mobile.iOS
{
    public partial class PlayerController : UIViewController
    {
        const String TIME_FORMAT = "{0:mm\\:ss}";

        AVAudioPlayer _player;
        NSTimer _uIUpdateTimer;

        public PlayerController (IntPtr handle)
            : base (handle)
        {
        }

        public override void AwakeFromNib ()
        {
            base.AwakeFromNib ();
//            var fileUrl = NSBundle.MainBundle.PathForResource ("sample", "m4a");
//            _player = AVAudioPlayer.FromUrl (new NSUrl (fileUrl, false));
//            _player.FinishedPlaying += (sender, e) =>
//            {
//                if (!e.Status)
//                {
//                    Console.WriteLine ("Did not complete successfully");
//                }
//                _player.CurrentTime = 0;
//                UpdateViewForPlayerState ();
//            };

            SetSharedInstanceForRemoteControl ();
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            _playerProgressBar.Value = 0;
            _title.Text = AppDelegate.Self.CurrentEpisode.Title;
        }

        #region UI Action Responder

        partial void _playOrPauseButton_TouchUpInside (UIButton sender)
        {
            if (_player.Playing)
            {
                Pause ();
            }
            else
            {
                Play ();
            }
        }

        partial void _playerProgressBar_ValueChanged (UISlider sender)
        {
            _player.CurrentTime = sender.Value;
            UpdateCurrentTime ();
        }

        #endregion

        void Pause ()
        {
            _player.Pause ();
            UpdateViewForPlayerState ();
        }

        void Play ()
        {
            if (_player.Play ())
            {
                UpdateViewForPlayerState ();
            }
            else
            {
                Console.WriteLine ("Could not play the file {0}", _player.Url);
            }
        }

        void UpdateViewForPlayerState ()
        {
            UpdateCurrentTime ();

            if (_uIUpdateTimer != null)
            {
                _uIUpdateTimer.Invalidate ();
            }

            if (_player.Playing)
            {
                // TODO update PlayOrPauseButton
                _uIUpdateTimer = NSTimer.CreateRepeatingScheduledTimer (
                    TimeSpan.FromSeconds (0.01),
                    obj => UpdateCurrentTime ()
                );
            }
            else 
            {
                // TODO update PlayOrPauseButton
                _uIUpdateTimer = null;
            }
        }

        void UpdateCurrentTime ()
        {
            UpdateTimerTextViews ((int) _player.Duration, (int) _player.CurrentTime);
            _playerProgressBar.Value = (float) _player.CurrentTime;
            _playerProgressBar.MaxValue = (float) _player.Duration;
        }

        void UpdateTimerTextViews (int progressMax, int progress)
        {
            _spentTime.Text = String.Format (TIME_FORMAT, TimeSpan.FromSeconds (progress));
            _leftTime.Text = String.Format (
                TIME_FORMAT,
                TimeSpan.FromSeconds (progressMax - progress)
            );
        }

        #region Remote Control

        void SetSharedInstanceForRemoteControl ()
        {
            var sharedInstance = AVAudioSession.SharedInstance ();
            sharedInstance.SetCategory (AVAudioSessionCategory.Playback);
            sharedInstance.SetActive (true);
        }

        public override bool CanBecomeFirstResponder
        {
            get
            {
                return true;
            }
        }

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);

            UIApplication.SharedApplication.BeginReceivingRemoteControlEvents ();
            BecomeFirstResponder ();
        }

        public override void ViewWillDisappear (bool animated)
        {
            base.ViewWillDisappear (animated);

            UIApplication.SharedApplication.EndReceivingRemoteControlEvents ();
            ResignFirstResponder ();
        }

        public override void RemoteControlReceived (UIEvent theEvent)
        {
            base.RemoteControlReceived (theEvent);

            if (theEvent.Type == UIEventType.RemoteControl)
            {
                switch (theEvent.Subtype)
                {
                    case UIEventSubtype.RemoteControlPlay:
                        Play ();
                        break;
                    case UIEventSubtype.RemoteControlPause:
                        Pause ();
                        break;
                    case UIEventSubtype.RemoteControlTogglePlayPause:
                        if (_player.Playing)
                        {
                            Pause ();
                        }
                        else
                        {
                            Play ();
                        }
                        break;
                }
            }
        }

        #endregion
    }
}

 