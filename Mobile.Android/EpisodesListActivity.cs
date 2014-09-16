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
using System.Collections.Generic;

namespace DrunkAudible.Mobile.Android
{
    [Activity (Label = "EpisodesListViewActivity")]
    public class EpisodesListActivity : ListActivity
    {
        Album _album;

        const String DEBUG_TAG = "EpisodesListActivity";

        const String ALBUM_ID_INTENT_EXTRA = "AlbumID";

        List<int> _audioDownloadsInProgress = new List<int> ();

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            SetContentView (Resource.Layout.AudioListView);

            InitializeExtrasFromIntent ();

            ListAdapter = new AudioListAdapter (this, _album);

            ListView.ItemClick += OnAlbumItemClicked;
        }

        public static Intent CreateIntent (Context context, int albumID)
        {
            var intent = new Intent (context, typeof(EpisodesListActivity));
            intent.PutExtra (ALBUM_ID_INTENT_EXTRA, albumID);
            return intent;
        }

        void InitializeExtrasFromIntent ()
        {
            if (Intent.HasExtra (ALBUM_ID_INTENT_EXTRA))
            {
                var albumID = Intent.GetIntExtra (ALBUM_ID_INTENT_EXTRA, -1);
                _album = DatabaseSingleton.Orm.Albums.FirstOrDefault (a => a.ID == albumID);
            }
        }

        async void OnAlbumItemClicked (object sender, AdapterView.ItemClickEventArgs e)
        {
            var selectedEpisode = _album.Episodes [e.Position];

            if (_audioDownloadsInProgress.Contains (selectedEpisode.ID))
            {
                return;
            }

            if (AudioDownloader.HasLocalFile (selectedEpisode.RemoteURL))
            {
                StartActivity (AudioPlayerActivity.CreateIntent (this, _album.ID, selectedEpisode.ID));
            }
            else
            {
                _audioDownloadsInProgress.Add (selectedEpisode.ID);
                await StartDownloadAsync (sender, e).ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            _audioDownloadsInProgress.Remove (selectedEpisode.ID);
                        }
                    }
                );
            }
        }

        async Task StartDownloadAsync(object sender, AdapterView.ItemClickEventArgs e)
        {
            var view = ListView.GetChildAt (e.Position);
            var progressBar = view.FindViewById<ProgressBar> (Resource.Id.DownloadProgress);
            progressBar.Progress = 0;
            var progressReporter = new Progress<DownloadBytesProgress> ();
            progressReporter.ProgressChanged += 
                (s, args) => progressBar.Progress = (int) (progressBar.Max * args.PercentComplete);
            var selectedEpisode = _album.Episodes [e.Position];

            var downloadTask = AudioDownloader.CreateDownloadTask (selectedEpisode.RemoteURL, progressReporter);
            var bytesDownloaded = await downloadTask;
            Log.Debug (DEBUG_TAG, "Downloaded {0} bytes.", bytesDownloaded);
        }
    }
}

