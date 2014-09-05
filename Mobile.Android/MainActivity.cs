// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DrunkAudible.Data;
using Android.Util;
using DrunkAudible.Data.Models;

namespace Mobile.Android
{
    [Activity (Label = "Faves", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ListActivity {
        AudioListAdapter _adapter;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate (savedInstanceState);

            _adapter = new AudioListAdapter (this, DatabaseSingleton.Orm.Albums);

            ListAdapter = _adapter;

            ListView.ItemClick += OnAlbumItemClicked_GoTo_EpisodesListViewActivity;
        }

        private void OnAlbumItemClicked_GoTo_EpisodesListViewActivity(object sender, AdapterView.ItemClickEventArgs e)
        {
            var selectedAlbum = DatabaseSingleton.Orm.Albums [e.Position];
            var activity = new Intent (this, typeof(EpisodesListActivity));
            activity.PutExtra ("AlbumID", selectedAlbum.ID);

            StartActivity (activity);
        }
    }
}

