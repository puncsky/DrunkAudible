// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DrunkAudible.Data.Models;
using System.Threading.Tasks;
using Android.Util;

namespace DrunkAudible.Mobile.Android
{
    [Activity (Label = "EpisodesListViewActivity")]
    public class EpisodesListActivity : ListActivity
    {
        AudioEpisode[] _episodes;

        const String DEBUG_TAG = "EpisodesListActivity";

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            SetContentView (Resource.Layout.AudioListView);

            var albumID = Intent.GetIntExtra ("AlbumID", -1);
            var album = DatabaseSingleton.Orm.Albums.FirstOrDefault (a => a.ID == albumID);
            _episodes = album.Episodes.ToArray ();

            ListAdapter = new AudioListAdapter (this, album);

            ListView.ItemClick += OnAlbumItemClicked;
        }

        async void OnAlbumItemClicked (object sender, AdapterView.ItemClickEventArgs e)
        {
            var clickedEpisode = _episodes [e.Position];
            if (AudioDownloader.HasLocalFile(clickedEpisode.RemoteURL))
            {
                StartActivity (new Intent (this, typeof (AudioPlayerActivity)));
            }

            await StartDownloadAsync (sender, e);
        }

        async Task StartDownloadAsync(object sender, AdapterView.ItemClickEventArgs e)
        {
            var view = ListView.GetChildAt (e.Position);
            var progressBar = view.FindViewById<ProgressBar> (Resource.Id.DownloadProgress);
            progressBar.Progress = 0;
            var progressReporter = new Progress<DownloadBytesProgress> ();
            progressReporter.ProgressChanged += (s, args) => progressBar.Progress = (int) (100 * args.PercentComplete);
            var clickedEpisode = _episodes [e.Position];

            var downloadTask = AudioDownloader.CreateDownloadTask (clickedEpisode.RemoteURL, progressReporter);
            var bytesDownloaded = await downloadTask;
            Log.Debug (DEBUG_TAG, "Downloaded {0} bytes.", bytesDownloaded);
        }
    }
}

