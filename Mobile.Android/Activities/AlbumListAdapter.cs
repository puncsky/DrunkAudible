﻿// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Android.Content;
using Android.Views;
using DrunkAudible.Data.Models;
using Android.Widget;

namespace DrunkAudible.Mobile.Android
{
    public class AlbumListAdapter : IconAndTitleItemListAdapter
    {
        readonly Album [] _albums;

        public AlbumListAdapter (Context context, Album [] albums)
            : base (context, albums)
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

            return iconAndTitleView;
        }
    }
}
