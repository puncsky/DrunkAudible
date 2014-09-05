﻿// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DrunkAudible.Data;

namespace Mobile.Android
{
    [Activity (Label = "EpisodesListViewActivity")]
    public class EpisodesListActivity : ListActivity
    {
        AudioListAdapter _adapter;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate (savedInstanceState);

            var albumID = Intent.GetIntExtra ("AlbumID", -1);
            var album = DatabaseSingleton.Orm.Albums.FirstOrDefault (a => a.ID == albumID);
            var episodes = album.Episodes.ToArray ();
            foreach (var e in episodes) {
                if (e.Authors == null) {
                    e.Authors = album.Authors;
                    e.Narrator = album.Narrator;
                }
            }
            _adapter = new AudioListAdapter (this, episodes);

            ListAdapter = _adapter;

            ListView.ItemClick += OnAlbumItemClicked_;
        }

        private void OnAlbumItemClicked_(object sender, AdapterView.ItemClickEventArgs e)
        {
        }
    }
}

