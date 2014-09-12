// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DrunkAudible.Data.Models;

namespace DrunkAudible.Mobile.Android
{
    [Activity (Label = "Faves", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ListActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            SetContentView (Resource.Layout.AudioListView);

            ListAdapter = new AudioListAdapter (this, DatabaseSingleton.Orm.Albums);

            ListView.ItemClick += OnAlbumItemClicked_GoTo_EpisodesListViewActivity;
        }

        void OnAlbumItemClicked_GoTo_EpisodesListViewActivity(object sender, AdapterView.ItemClickEventArgs e)
        {
            var selectedAlbum = DatabaseSingleton.Orm.Albums [e.Position];
            var activity = new Intent (this, typeof(EpisodesListActivity));
            activity.PutExtra ("AlbumID", selectedAlbum.ID);

            StartActivity (activity);
        }
    }
}

