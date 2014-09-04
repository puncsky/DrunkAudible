// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;
using DrunkAudible.Data.Models;
using DrunkAudible.Data.Models.JoinTables;

namespace DrunkAudible.Data
{
    public static class DatabaseInitializer
    {
        #region Sample Data

        // Models

        public static Author[] AuthorSamples {
            get {
                return new[] {
                    new Author {
                        ID = 1,
                        Name = "金庸",
                    },
                    new Author {
                        ID = 2,
                        Name = "高晓松",
                    },
                    new Author {
                        ID = 3,
                        Name = "梁冬",
                    },
                    new Author {
                        ID = 4,
                        Name = "吴伯凡",
                    },
                };
            }
        }

        public static Album[] AlbumSamples {
            get {
                return new[] {
                    new Album {
                        ID = 1,
                        Title = "天龙八部",
                        Authors = new[] { new Author { Name = "金庸" } },
                        Narrator = "倪清",
                    },
                    new Album {
                        ID = 2,
                        Title = "冬吴相对论",
                        Authors = new[] { new Author { Name = "梁冬" }, new Author { Name = "吴伯凡" } },
                    },
                    new Album {
                        ID = 3,
                        Title = "笑傲江湖",
                        Authors = new[] { new Author { Name = "金庸" } },
                    },
                };
            }
        }

        public static AudioEpisode[] EpisodeSamples {
            get {
                return new[] {
                    new AudioEpisode {
                        ID = 1,
                        Title = "青衫磊落险峰行",
                        PartIndex = 1,
                        RemoteURL = "http://115.28.189.40/tingguo/novel/tlbb/tlbb1.flab",
                        Duration = 5589.468,
                        Authors = new[] { new Author { ID = 1, Name = "金庸" } },
                        Narrator = "倪清",
                        Album = AlbumSamples.Where (s => s.Title == "天龙八部").FirstOrDefault (),
                    },
                    new AudioEpisode {
                        ID = 2,
                        PartIndex = 451,
                        Title = "财商教育",
                        RemoteURL = "http://fruitlab.net/tinggo/tuokouxiu/dwxdl/dwxdl_451.flab",
                        Duration = 1509.222,
                        Authors = new[] {
                            new Author { ID = 3, Name = "梁冬" },
                            new Author { ID = 4, Name = "吴伯凡" }
                        },
                        Album = AlbumSamples.Where (s => s.Title == "冬吴相对论").FirstOrDefault (),
                    },
                };
            }
        }

        public static User[] UserSamples {
            get {
                return new[] {
                    new User {
                        ID = 1,
                        Alias = "Puncsky",
                    },
                    new User {
                        ID = 2,
                        Alias = "Dobby",
                    },
                };
            }
        }

        // JoinTables

        public static AudioEpisodesToAlbum[] AudioEpisodesToAlbumSamples {
            get {
                return new[] {
                    new AudioEpisodesToAlbum {
                        EpisodeID = 1,
                        AlbumID = 1,
                    },
                    new AudioEpisodesToAlbum {
                        EpisodeID = 2,
                        AlbumID = 2,
                    },
                };
            }
        }

        public static AlbumToAuthors[] AlbumToAuthorsSamples {
            get {
                return new[] {
                    new AlbumToAuthors {
                        AlbumID = 1,
                        AuthorID = 1,
                    },
                    new AlbumToAuthors {
                        AlbumID = 2,
                        AuthorID = 3,
                    },
                    new AlbumToAuthors {
                        AlbumID = 2,
                        AuthorID = 4,
                    },
                    new AlbumToAuthors {
                        AlbumID = 3,
                        AuthorID = 1,
                    },
                };
            }
        }

        #endregion

        public static void Initialize(SQLiteConnection database) {
            AddAuthors (database);
            AddAlbum (database);
            AddEpisodes (database);
            AddUsers (database);
        }

        private static void AddAuthors(SQLiteConnection database) {
            database.CreateTable<Author> ();

            if (database.Table<Author> ().Count () == 0) {
                database.InsertAll (AuthorSamples);
            }
        }

        private static void AddAlbum(SQLiteConnection database) {
            database.CreateTable<Album> ();

            if (database.Table<Album> ().Count () == 0) {
                database.InsertAll (AlbumSamples);
            }
        }

        private static void AddEpisodes(SQLiteConnection database) {
            database.CreateTable<AudioEpisode> ();

            if (database.Table<AudioEpisode> ().Count () == 0) {
                database.InsertAll (EpisodeSamples);
            }
        }

        private static void AddUsers(SQLiteConnection database) {
            database.CreateTable<User> ();

            if (database.Table<User> ().Count () == 0) {
                database.InsertAll (UserSamples);
            }
        }
    }
}

