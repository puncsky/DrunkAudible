// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DrunkAudible.Data.Models;

namespace DrunkAudible.Mobile.Android
{
    [Activity (Label = "AudioListActivity")]
    public class AudioListActivity : ListActivity
    {
        Album _album;

        const String DEBUG_TAG = "AudioListActivity";
        const String ALBUM_ID_INTENT_EXTRA = "AlbumID";

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
            var intent = new Intent (context, typeof(AudioListActivity));
            intent.PutExtra (ALBUM_ID_INTENT_EXTRA, albumID);
            return intent;
        }

        void InitializeExtrasFromIntent ()
        {
            if (Intent.HasExtra (ALBUM_ID_INTENT_EXTRA))
            {
                var albumID = Intent.GetIntExtra (ALBUM_ID_INTENT_EXTRA, -1);
                _album = DatabaseSingleton.Orm.Albums.FirstOrDefault (a => a.Id == albumID);
            }
        }

        async void OnAlbumItemClicked (object sender, AdapterView.ItemClickEventArgs e)
        {
            var selectedEpisode = _album.Episodes [e.Position];

            if (AndroidAudioDownloader.ViewsDownloadInProgressByAudioId.ContainsKey (selectedEpisode.RemoteURL))
            {
                return;
            }

            if (AudioDownloader.HasLocalFile (selectedEpisode.RemoteURL, selectedEpisode.FileSize))
            {
                var resultIntent = new Intent ();
                resultIntent.PutExtra (AudioPlayerFragment.EPISODE_ID_INTENT_EXTRA, selectedEpisode.Id);
                resultIntent.PutExtra (AudioPlayerFragment.ALBUM_ID_INTENT_EXTRA, _album.Id);
                SetResult (Result.Ok, resultIntent);
                Finish ();
            }
            else
            {
                await AndroidAudioDownloader.StartDownloadAsync (e.Position, selectedEpisode.RemoteURL, ListView);
            }
        }
    }
}

