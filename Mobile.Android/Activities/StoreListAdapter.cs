// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using RestSharp;
using DrunkAudible.Data.Models;

namespace DrunkAudible.Mobile.Android
{
    public class StoreListAdapter : IconAndTitleItemListAdapter
    {
        readonly List<Album> _albums;

        public StoreListAdapter (Context context, List<Album> albums)
            : base (context, albums.ToList<IIconAndTitleItem> (), Resource.Layout.StoreProductListViewElement)
        {
            _albums = albums;
        }

        public override View GetView (int position, View convertView, ViewGroup parent)
        {
            var iconAndTitleView = base.GetView (position, convertView, parent);
            var album = _albums [position];

            iconAndTitleView.SetNarratorsAndAuthors (album);

            var description = iconAndTitleView.FindViewById<TextView> (Resource.Id.Description);
            description.Text = album.Description;

            var starred = iconAndTitleView.FindViewById<TextView> (Resource.Id.Starred);
            IconProvider.ConvertTextViewToIcon (Context.Assets, starred);
            if (DrunkAudibleApplication.Self.Database.Albums.Any (a => a.Id == album.Id))
            {
                starred.Visibility = ViewStates.Visible;
            }

            iconAndTitleView.Click += (sender, e) =>
            {
                if (starred.Visibility != ViewStates.Visible)
                {
                    starred.Visibility = ViewStates.Visible;
                    Toast.MakeText (Context, Resource.String.AddedToFaves, ToastLength.Short).Show ();
                    LoadRemoteEpisodesAsync (album);
                }
            };

            return iconAndTitleView;
        }

        static void LoadRemoteEpisodesAsync (Album album)
        {
            Task.Factory.StartNew (() => LoadRemoteEpisodes (album));
        }

        static void LoadRemoteEpisodes (Album album)
        {
            var client = new WebServiceClient ();
            var request = new WebServiceRequest ("/albums/" + album.Id + "/episodes", Method.GET);
            var response = client.Execute (request);
            var episodes = JsonConvert.DeserializeObject<List<AudioEpisode>> (response.Content);
            album.Episodes = episodes;
            if (DrunkAudibleApplication.Self.Database.Albums.All (a => a.Id != album.Id))
            {
                DrunkAudibleApplication.Self.Database.InsertOrUpdate (album);
                DrunkAudibleApplication.Self.Database.Albums.Add (album);
            }
        }
    }
}

