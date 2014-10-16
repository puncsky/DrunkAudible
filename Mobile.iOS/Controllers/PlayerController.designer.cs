// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using Foundation;
using UIKit;
using System.CodeDom.Compiler;

namespace Mobile.iOS
{
	[Register ("PlayerController")]
	partial class PlayerController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel _leftTime { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISlider _playerProgressBar { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton _playOrPauseButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel _spentTime { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel _title { get; set; }

		[Action ("_playerProgressBar_ValueChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void _playerProgressBar_ValueChanged (UISlider sender);

		[Action ("_playOrPauseButton_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void _playOrPauseButton_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (_leftTime != null) {
				_leftTime.Dispose ();
				_leftTime = null;
			}
			if (_playerProgressBar != null) {
				_playerProgressBar.Dispose ();
				_playerProgressBar = null;
			}
			if (_playOrPauseButton != null) {
				_playOrPauseButton.Dispose ();
				_playOrPauseButton = null;
			}
			if (_spentTime != null) {
				_spentTime.Dispose ();
				_spentTime = null;
			}
			if (_title != null) {
				_title.Dispose ();
				_title = null;
			}
		}
	}
}
