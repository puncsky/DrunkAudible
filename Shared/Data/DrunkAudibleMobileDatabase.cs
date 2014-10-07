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
    public class DrunkAudibleMobileDatabase : SQLiteConnection
    {
        public static String DATABASE_FILE_NAME = "DrunkAudible.Mobile.SQLite.db3";

        Album[] _albums;

        public DrunkAudibleMobileDatabase()
            : base (DatabasePath)
        {
            CreateTable<Author> ();

            CreateTable<Album> ();
            CreateTable<AlbumToAuthors> ();

            CreateTable<AudioEpisode> ();
            CreateTable<AudioEpisodesToAlbum> ();

            CreateTable<User> ();
        }

        public new static String DatabasePath
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
            CreateTable<Album> ();
            InsertOrReplace (album);

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
            DeleteAll<Author> ();

            DeleteAll<Album> ();
            DeleteAll<AlbumToAuthors> ();

            DeleteAll<AudioEpisode> ();
            DeleteAll<AudioEpisodesToAlbum> ();

            DeleteAll<User> ();
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
                var albums = Table<Album> ().ToArray<Album> ();

                LoadAuthors (albums);
                LoadEpisodes (albums);

                _albums = albums;

                return _albums;
            }
        }

        void SaveAlbumAuthors (Author[] authors, int albumID)
        {
            // Update related entries in the join table.
            CreateTable<AlbumToAuthors> ();
            var oldEntries = Table<AlbumToAuthors> ().Where (a => a.AlbumId == albumID).ToArray ();
            foreach (var oldEntry in oldEntries)
            {
                Delete (oldEntry);
            }
            foreach (var author in authors)
            {
                Insert (new AlbumToAuthors ()
                    {
                        AlbumId = albumID,
                        AuthorId = author.Id,
                    });
            }

            // Insert or update the related table for the ignored property Author.
            CreateTable<Author> ();
            foreach (var author in authors)
            {
                InsertOrReplace (author);
            }
        }

        void SaveAlbumEpisodes (AudioEpisode[] episodes, int albumID)
        {
            // Update related entries in the join table.
            CreateTable<AudioEpisodesToAlbum> ();
            var oldEntries = Table<AudioEpisodesToAlbum> ().Where (a => a.AlbumId == albumID).ToArray ();
            foreach (var oldEntry in oldEntries)
            {
                Delete (oldEntry);
            }
            foreach (var episode in episodes)
            {
                Insert (new AudioEpisodesToAlbum
                    {
                        AlbumId = albumID,
                        EpisodeId = episode.Id,
                    }
                );
            }

            CreateTable<AudioEpisode> ();
            foreach (var episode in episodes)
            {
                InsertOrReplace (episode);
            }
        }

        void LoadAuthors (Album[] albums)
        {
            var joinTableLookUp = 
                Table<AlbumToAuthors> ()
                .ToLookup (a => a.AlbumId);

            var authorsDictionary = 
                Table<Author> ()
                .ToDictionary (a => a.Id);

            // Set up the related property Author, because Album table in the database ingored it.
            foreach (var album in albums)
            {
                var relatedAuthorIDs = joinTableLookUp[album.Id].Select(i => i.AuthorId);

                album.Authors = relatedAuthorIDs.Select (r => authorsDictionary [r]).ToArray ();
            }
        }

        void LoadEpisodes (Album[] albums)
        {
            var joinTableLookUp = 
                Table<AudioEpisodesToAlbum> ()
                .ToLookup (a => a.AlbumId);

            var episodesDictionary = 
                Table<AudioEpisode> ()
                .ToDictionary (a => a.Id);

            // Set up the related property Author, because Album table in the database ingored it.
            foreach (var album in albums)
            {
                var relatedEpisodeIDs = joinTableLookUp[album.Id].Select(i => i.EpisodeId);

                album.Episodes = relatedEpisodeIDs.Select (r => episodesDictionary [r]).ToArray ();
            }
        }
    }
}

