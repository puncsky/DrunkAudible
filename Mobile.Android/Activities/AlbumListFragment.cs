// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.IO;
using Android.App;
using Android.OS;
using DrunkAudible.Data;
using DrunkAudible.Data.Models;
using Android.Views;
using Android.Content;

namespace DrunkAudible.Mobile.Android
{
    public class AlbumListFragment : ListFragment
    {
        const int SELECT_AUDIO_ACTIVITY_RESULT = 1;

        Album _currentAlbum;
        AudioEpisode _currentEpisode;

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView (inflater, container, savedInstanceState);

            SetAssetDatabase ();

            _currentAlbum = ExtraUtils.GetAlbum (Activity.Intent);
            _currentEpisode = ExtraUtils.GetAudioEpisode (Activity.Intent, _currentAlbum);
            ListAdapter = new AlbumListAdapter (Activity, DatabaseSingleton.Orm.Albums, _currentAlbum);

            return inflater.Inflate(Resource.Layout.AudioListView, container, false);
        }

        public override void OnActivityCreated (Bundle savedInstanceState)
        {
            base.OnActivityCreated (savedInstanceState);

            ListView.ItemClick += (sender, e) =>
            {
                var selectedAlbum = DatabaseSingleton.Orm.Albums [e.Position];
                StartActivityForResult (
                    AudioListActivity.CreateIntent (
                        Activity,
                        selectedAlbum.Id, _currentEpisode != null ? _currentEpisode.Id : -1
                    ),
                    SELECT_AUDIO_ACTIVITY_RESULT
                );
            };

            if (_currentAlbum != null)
            {
                ListView.SetSelection (Array.IndexOf(DatabaseSingleton.Orm.Albums, _currentAlbum));
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

        #region SetDatabase

        // TODO move to splash screen activity
        void SetAssetDatabase()
        {
            var docFolder = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
            var dbFile = Path.Combine (docFolder, ObjectRelationalMapping.DATABASE_FILE_NAME);
            if (!File.Exists (dbFile))
            {
                var s = Activity.Assets.Open(ObjectRelationalMapping.DATABASE_FILE_NAME);  // DATA FILE RESOURCE ID
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

