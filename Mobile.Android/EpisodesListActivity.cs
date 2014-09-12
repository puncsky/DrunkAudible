// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace DrunkAudible.Mobile.Android
{
    [Activity (Label = "EpisodesListViewActivity")]
    public class EpisodesListActivity : ListActivity
    {
        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            SetContentView (Resource.Layout.AudioListView);

            var albumID = Intent.GetIntExtra ("AlbumID", -1);
            var album = DatabaseSingleton.Orm.Albums.FirstOrDefault (a => a.ID == albumID);
            var episodes = album.Episodes.ToArray ();
            foreach (var e in episodes)
            {
                if (e.Authors == null)
                {
                    e.Authors = album.Authors;
                    e.Narrator = album.Narrator;
                }
            }

            ListAdapter = new AudioListAdapter (this, episodes);

            ListView.ItemClick += OnAlbumItemClicked_;
        }

        void OnAlbumItemClicked_(object sender, AdapterView.ItemClickEventArgs e)
        {
            StartActivity (new Intent (this, typeof(AudioPlayerActivity)));
        }
    }
}

