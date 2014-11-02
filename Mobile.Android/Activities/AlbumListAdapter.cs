// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;
using DrunkAudible.Data.Models;

namespace DrunkAudible.Mobile.Android
{
    public class AlbumListAdapter : IconAndTitleItemListAdapter
    {
        readonly List<Album> _albums;

        public AlbumListAdapter (Context context, List<Album> albums)
            : base (context, albums.ToList<IIconAndTitleItem> (), Resource.Layout.AudioListViewElement)
        {
            _albums = albums;
        }

        public override View GetView (int position, View convertView, ViewGroup parent)
        {
            var iconAndTitleView = base.GetView (position, convertView, parent);
            var album = _albums [position];

            iconAndTitleView.SetNarratorsAndAuthors (album);

            // TODO #17 Set the album entry progress bar as indicator of listening progress for the whole album.
            var progressBar = iconAndTitleView.FindViewById<ProgressBar> (Resource.Id.DownloadProgress);
            progressBar.Visibility = ViewStates.Gone;

            if (album == DrunkAudibleApplication.Self.CurrentAlbum)
            {
                var isPlayingIndicator = iconAndTitleView.FindViewById<TextView> (Resource.Id.IsPlayingIndicator);
                IconProvider.ConvertTextViewToIcon (Context.Assets, isPlayingIndicator);
                isPlayingIndicator.Visibility = ViewStates.Visible;
            }

            return iconAndTitleView;
        }
    }
}

