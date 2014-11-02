// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Newtonsoft.Json;
using RestSharp;
using DrunkAudible.Data.Models;

namespace DrunkAudible.Mobile.Android
{
    public class StoreFragment : ListFragment
    {
        List<Album> _albums;
        bool _isGettingRemoteAlbums;

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView (inflater, container, savedInstanceState);

            return inflater.Inflate(Resource.Layout.StoreView, container, false);
        }

        public override void OnActivityCreated (Bundle savedInstanceState)
        {
            base.OnActivityCreated (savedInstanceState);

            if (_albums != null)
            {
                ListAdapter = new StoreListAdapter (Activity, _albums);
            }
            else if (!_isGettingRemoteAlbums)
            {
                GetRemoteAlbumsAsync ();
            }
        }

        void GetRemoteAlbumsAsync ()
        {
            Task.Factory
                .StartNew (() => GetRemoteAlbums ())
                .ContinueWith (t =>
                    {
                        if (!t.IsFaulted && t.Result != null && Activity != null)
                        {
                            _albums = t.Result;
                            ListAdapter = new StoreListAdapter (Activity, _albums);
                        }
                        _isGettingRemoteAlbums = false;
                    },
                    TaskScheduler.FromCurrentSynchronizationContext ()
                );
        }

        List<Album> GetRemoteAlbums ()
        {
            _isGettingRemoteAlbums = true;
            var client = new WebServiceClient ();
            var request = new WebServiceRequest ("/albums", Method.GET);
            var response = client.Execute (request);
            return JsonConvert.DeserializeObject<List<Album>> (response.Content);
        }
    }
}

