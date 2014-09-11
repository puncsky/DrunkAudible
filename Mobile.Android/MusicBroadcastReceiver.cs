// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Android.App;
using Android.Content;
using Android.Media;

namespace Mobile.Android
{
    /// <summary>
    /// This is a simple intent receiver that is used to stop playback
    /// when audio become noisy, such as the user unplugged headphones
    /// </summary>
    [BroadcastReceiver]
    [IntentFilter (new[] { AudioManager.ActionAudioBecomingNoisy })]
    public class MusicBroadcastReceiver: BroadcastReceiver
    {
        public override void OnReceive (Context context, Intent intent)
        {
            if (intent.Action != AudioManager.ActionAudioBecomingNoisy)
            {
                return;
            }

            //signal the service to stop!
            var stopIntent = new Intent (StreamingBackgroundService.ACTION_STOP);
            context.StartService (stopIntent);
        }
    }
}

