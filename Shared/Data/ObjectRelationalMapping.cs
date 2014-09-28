// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DrunkAudible.Data.Models;
using DrunkAudible.Data.Models.JoinTables;
using SQLite;

namespace DrunkAudible.Data
{
    public class ObjectRelationalMapping
    {
        public static String DATABASE_FILE_NAME = "DrunkAudible.Mobile.SQLite.db3";

        readonly SQLiteConnection _database;

        Album[] _albums;

        public ObjectRelationalMapping()
        {
            _database = new SQLiteConnection (DatabasePath);

            _database.CreateTable<Author> ();

            _database.CreateTable<Album> ();
            _database.CreateTable<AlbumToAuthors> ();

            _database.CreateTable<AudioEpisode> ();
            _database.CreateTable<AudioEpisodesToAlbum> ();

            _database.CreateTable<User> ();
        }

        String DatabasePath
        {
            get
            { 
                #if __IOS__
                string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
                string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
                var path = Path.Combine(libraryPath, DATABASE_FILE_NAME);
                #elif __ANDROID__
                string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
                var path = Path.Combine (documentsPath, DATABASE_FILE_NAME);
                #elif WINDOWS_PHONE
                var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, DATABASE_FILE_NAME);
                #else
                string localApplicationData = Environment.CurrentDirectory;
                var path = Path.Combine(localApplicationData, DATABASE_FILE_NAME);
                #endif

                return path;
            }
        }

        public void InsertOrUpdate (Album album)
        {
            Database.CreateTable<Album> ();
            Database.InsertOrReplace (album);

            // Update all the related tables, because the Album instance holds the reference to the Authors collection
            // object that cannot be stored directly into the same table.
            if (album.Authors != null)
            {
                SaveAlbumAuthors (album.Authors.ToArray (), album.Id);

            }
            if (album.Episodes != null)
            {
                SaveAlbumEpisodes (album.Episodes.ToArray (), album.Id);
            }
        }

        public void InsertOrUpdate (Album[] albums)
        {
            foreach (var album in albums)
            {
                InsertOrUpdate (album);
            }
        }

        public void ClearAll ()
        {
            _database.DeleteAll<Author> ();

            _database.DeleteAll<Album> ();
            _database.DeleteAll<AlbumToAuthors> ();

            _database.DeleteAll<AudioEpisode> ();
            _database.DeleteAll<AudioEpisodesToAlbum> ();

            _database.DeleteAll<User> ();
        }

        public Album[] Albums
        { 
            get
            {
                if (_albums == null)
                {
                    _albums = LoadAlbums;
                }

                return _albums;
            }
        }

        public Album[] LoadAlbums
        { 
            get
            {
                var albums = Database.Table<Album> ().ToArray<Album> ();

                LoadAuthors (albums);
                LoadEpisodes (albums);

                _albums = albums;

                return _albums;
            }
        }

        public SQLiteConnection Database
        {
            get
            {
                return _database;
            }
        }

        void SaveAlbumAuthors (Author[] authors, int albumID)
        {
            // Update related entries in the join table.
            Database.CreateTable<AlbumToAuthors> ();
            var oldEntries = Database.Table<AlbumToAuthors> ().Where (a => a.AlbumId == albumID).ToArray ();
            foreach (var oldEntry in oldEntries)
            {
                Database.Delete (oldEntry);
            }
            foreach (var author in authors)
            {
                Database.Insert (new AlbumToAuthors ()
                    {
                        AlbumId = albumID,
                        AuthorId = author.Id,
                    });
            }

            // Insert or update the related table for the ignored property Author.
            Database.CreateTable<Author> ();
            foreach (var author in authors)
            {
                Database.InsertOrReplace (author);
            }
        }

        void SaveAlbumEpisodes (AudioEpisode[] episodes, int albumID)
        {
            // Update related entries in the join table.
            Database.CreateTable<AudioEpisodesToAlbum> ();
            var oldEntries = Database.Table<AudioEpisodesToAlbum> ().Where (a => a.AlbumId == albumID).ToArray ();
            foreach (var oldEntry in oldEntries)
            {
                Database.Delete (oldEntry);
            }
            foreach (var episode in episodes)
            {
                Database.Insert (new AudioEpisodesToAlbum ()
                    {
                        AlbumId = albumID,
                        EpisodeId = episode.Id,
                    });
            }

            Database.CreateTable<AudioEpisode> ();
            foreach (var episode in episodes)
            {
                Database.InsertOrReplace (episode);
            }
        }

        void LoadAuthors (Album[] albums)
        {
            var joinTableLookUp = Database
                .Table<AlbumToAuthors> ()
                .ToLookup (a => a.AlbumId);

            var authorsDictionary = Database
                .Table<Author> ()
                .ToDictionary (a => a.Id);

            // Set up the related property Author, because Album table in the database ingored it.
            foreach (var album in albums) {
                var relatedAuthorIDs = joinTableLookUp[album.Id].Select(i => i.AuthorId);

                album.Authors = relatedAuthorIDs.Select (r => authorsDictionary [r]).ToArray ();
            }
        }

        void LoadEpisodes (Album[] albums)
        {
            var joinTableLookUp = Database
                .Table<AudioEpisodesToAlbum> ()
                .ToLookup (a => a.AlbumId);

            var episodesDictionary = Database
                .Table<AudioEpisode> ()
                .ToDictionary (a => a.Id);

            // Set up the related property Author, because Album table in the database ingored it.
            foreach (var album in albums) {
                var relatedEpisodeIDs = joinTableLookUp[album.Id].Select(i => i.EpisodeId);

                album.Episodes = relatedEpisodeIDs.Select (r => episodesDictionary [r]).ToArray ();
            }
        }
    }
}

