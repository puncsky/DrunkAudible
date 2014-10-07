// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Android.Content;
using Android.Views;
using Android.Widget;
using DrunkAudible.Data.Models;
using Android.App;

namespace DrunkAudible.Mobile.Android
{
    public class AudioListAdapter : IconAndTitleItemListAdapter
    {
        readonly Album _album;

        public AudioListAdapter (Context context, Album album)
            : base (context, album.Episodes)
        {
            _album = album;
        }

        public override View GetView (int position, View convertView, ViewGroup parent)
        {
            var episode = _album.Episodes[position];

            View iconAndTitleView;
            if (AndroidAudioDownloader.ViewsDownloadInProgressByAudioId.ContainsKey (episode.RemoteURL))
            {
                iconAndTitleView = AndroidAudioDownloader.ViewsDownloadInProgressByAudioId [episode.RemoteURL];
            }
            else
            {
                iconAndTitleView = base.GetView (position, convertView, parent);
                iconAndTitleView.SetNarratorsAndAuthors (episode.Authors == null ? (IManMadeItem) episode : _album);

                var downloadProgressBar = iconAndTitleView.FindViewById<ProgressBar> (Resource.Id.DownloadProgress);
                downloadProgressBar.Progress =
                    AudioDownloader.HasLocalFile(episode.RemoteURL, episode.FileSize) ? downloadProgressBar.Max : 0;
            }

            if (episode == ((DrunkAudibleApplication) (((Activity) Context).Application)).CurrentEpisode)
            {
                var isPlayingIndicator = iconAndTitleView.FindViewById<TextView> (Resource.Id.IsPlayingIndicator);
                IconProvider.ConvertTextViewToIcon (Context.Assets, isPlayingIndicator);
                isPlayingIndicator.Visibility = ViewStates.Visible;
            }

            return iconAndTitleView;
        }
    }
}

