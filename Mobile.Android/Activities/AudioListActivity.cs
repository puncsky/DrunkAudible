// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
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

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            SetContentView (Resource.Layout.AudioListView);
            _album = ExtraUtils.GetAlbum (DrunkAudibleApplication.Self.Database, Intent);
            Title = _album.Title;
            ListAdapter = new AudioListAdapter (this, _album);
            ListView.ItemClick += OnAlbumItemClicked;

            if (_album == DrunkAudibleApplication.Self.CurrentAlbum)
            {
                var currentEpisode = DrunkAudibleApplication.Self.CurrentEpisode;
                if (!AudioEpisode.IsNullOrEmpty (currentEpisode))
                {
                    ListView.SetSelection (_album.Episodes.IndexOf(currentEpisode));
                }
            }
        }

        public static Intent CreateIntent (Context context, int albumID, int episodeId = -1)
        {
            var intent = new Intent (context, typeof(AudioListActivity));
            ExtraUtils.PutAlbum (intent, albumID);
            if (episodeId != -1)
            {
                ExtraUtils.PutEpisode (intent, episodeId);
            }
            return intent;
        }

        async void OnAlbumItemClicked (object sender, AdapterView.ItemClickEventArgs e)
        {
            var selectedEpisode = _album.Episodes [e.Position];

            if (AndroidAudioDownloader.ViewsDownloadInProgressByAudioId.ContainsKey (selectedEpisode.RemoteUrl))
            {
                return;
            }

            if (AudioDownloader.HasLocalFile (selectedEpisode.RemoteUrl, selectedEpisode.FileSize))
            {
                var resultIntent = new Intent ();
                ExtraUtils.PutEpisode (resultIntent, selectedEpisode.Id);
                ExtraUtils.PutAlbum (resultIntent, _album.Id);
                ExtraUtils.PutSelectedTab (resultIntent, (int) MainActivity.TabTitle.Player);
                SetResult (Result.Ok, resultIntent);

                StartService (PlayerService.CreateIntent (
                        this,
                        PlayerService.ACTION_PLAY,
                        _album.Id,
                        selectedEpisode.Id
                    ));

                Finish ();
            }
            else
            {
                await AndroidAudioDownloader.StartDownloadAsync (e.Position, selectedEpisode.RemoteUrl, ListView);
            }
        }
    }
}

