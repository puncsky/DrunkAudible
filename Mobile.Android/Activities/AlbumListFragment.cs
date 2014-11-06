// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using DrunkAudible.Data.Models;

namespace DrunkAudible.Mobile.Android
{
    public class AlbumListFragment : ListFragment
    {
        const int SELECT_AUDIO_ACTIVITY_RESULT = 1;

        List<Album> _albums;

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView (inflater, container, savedInstanceState);

            return inflater.Inflate(Resource.Layout.AudioListView, container, false);
        }

        public override void OnActivityCreated (Bundle savedInstanceState)
        {
            base.OnActivityCreated (savedInstanceState);

            _albums = DrunkAudibleApplication.Self.Database.Albums;
            ListAdapter = new AlbumListAdapter (Activity, _albums);

            ListView.ItemClick += (sender, e) =>
            {
                var selectedAlbum = _albums [e.Position];
                StartActivityForResult (
                    AudioListActivity.CreateIntent (
                        Activity,
                        selectedAlbum.Id
                    ),
                    SELECT_AUDIO_ACTIVITY_RESULT
                );
            };

            var currentAlbum = DrunkAudibleApplication.Self.CurrentAlbum;
            if (!Album.IsNullOrEmpty (currentAlbum))
            {
                ListView.SetSelection (_albums.IndexOf(currentAlbum));
            }
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult (requestCode, resultCode, data); 
            switch (requestCode)
            { 
                case (SELECT_AUDIO_ACTIVITY_RESULT):
                    {
                        if (resultCode == Result.Ok)
                        {
                            Activity.Intent.PutExtras (data.Extras);
                            var mainActivity = Activity as MainActivity;
                            if (mainActivity != null)
                            {
                                mainActivity.GetTab (MainActivity.TabTitle.Player).Select();
                            }
                        }
                        break; 
                    }
            }
        }
    }
}

