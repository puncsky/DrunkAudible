using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DrunkAudible.Models;
using SQLite;

namespace DrunkAudible.Data
{
    public class ObjectRelationalMapping
    {
        SQLiteConnection _database;

        string DatabasePath {
            get { 
                var sqliteFilename = "DrunkAudible.Mobile.SQLite.db3";

                #if __IOS__
                string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
                string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
                var path = Path.Combine(libraryPath, sqliteFilename);
                #elif __ANDROID__
                string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
                var path = Path.Combine(documentsPath, sqliteFilename);
                #elif WINDOWS_PHONE
                var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);
                #else
                string localApplicationData = Environment.CurrentDirectory;
                var path = Path.Combine(localApplicationData, sqliteFilename);
                #endif

                return path;
            }
        }

        public ObjectRelationalMapping()
        {
            _database = new SQLiteConnection (DatabasePath);

        }

        public SQLiteConnection Database {
            get {
                return _database;
            }
        }
    }
}

