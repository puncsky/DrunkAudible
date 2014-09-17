// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.IO;
using Android.App;
using Android.OS;
using Android.Widget;
using DrunkAudible.Data;
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

            SetAssetDatabase ();

            ListAdapter = new AudioListAdapter (this, DatabaseSingleton.Orm.Albums);

            ListView.ItemClick += OnAlbumItemClicked_GoTo_EpisodesListViewActivity;
        }

        void OnAlbumItemClicked_GoTo_EpisodesListViewActivity(object sender, AdapterView.ItemClickEventArgs e)
        {
            var selectedAlbum = DatabaseSingleton.Orm.Albums [e.Position];
            StartActivity (EpisodesListActivity.CreateIntent (this, selectedAlbum.ID));
        }

        #region SetDatabase

        // TODO move to splash screen activity
        void SetAssetDatabase()
        {
            var docFolder = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
            var dbFile = Path.Combine (docFolder, ObjectRelationalMapping.DATABASE_FILE_NAME);
            if (!File.Exists (dbFile))
            {
                var s = Assets.Open(ObjectRelationalMapping.DATABASE_FILE_NAME);  // DATA FILE RESOURCE ID
                var writeStream = new FileStream (dbFile, FileMode.OpenOrCreate, FileAccess.Write);
                ReadWriteStream (s, writeStream);
            }
        }

        static void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            const int length = 256;
            var buffer = new Byte[length];
            var bytesRead = readStream.Read(buffer, 0, length);

            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, length);
            }
            readStream.Close();
            writeStream.Close();
        }

        #endregion
    }
}

