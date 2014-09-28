// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using NUnit.Framework;
using DrunkAudible.Data.Models;
using SQLite;

namespace DrunkAudible.Mobile.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        // Models

        public static Author[] AuthorSamples {
            get {
                return new[] {
                    new Author {
                        Id = 1,
                        Name = "金庸",
                    },
                    new Author {
                        Id = 2,
                        Name = "高晓松",
                    },
                    new Author {
                        Id = 3,
                        Name = "梁冬",
                    },
                    new Author {
                        Id = 4,
                        Name = "吴伯凡",
                    },
                };
            }
        }

        public static Album[] AlbumSamples {
            get {
                return new[] {
                    new Album {
                        Id = 1,
                        Title = "天龙八部",
                        Authors = new[] { new Author { Id = 1, Name = "金庸" } },
                        Narrator = "倪清",
                        Episodes = new[] {
                            new AudioEpisode {
                                Id = 1,
                                Title = "青衫磊落险峰行",
                                PartIndex = 1,
                                RemoteURL = "http://115.28.189.40/tingguo/novel/tlbb/tlbb1.flab",
                                Duration = 5589.468,
                            },
                        },
                    },
                    new Album {
                        Id = 2,
                        Title = "冬吴相对论",
                        Authors = new[] { new Author { Id = 3, Name = "梁冬" }, new Author { Id = 4, Name = "吴伯凡" } },
                        Episodes = new[] {
                            new AudioEpisode {
                                Id = 2,
                                PartIndex = 451,
                                Title = "财商教育",
                                RemoteURL = "http://fruitlab.net/tinggo/tuokouxiu/dwxdl/dwxdl_451.flab",
                                Duration = 1509.222,
                            },
                        },
                    },
                    new Album {
                        Id = 3,
                        Title = "笑傲江湖",
                        Authors = new[] { new Author { Id = 1, Name = "金庸" } },
                    },
                };
            }
        }

        public static AudioEpisode[] EpisodeSamples {
            get {
                return new[] {
                    new AudioEpisode {
                        Id = 1,
                        Title = "青衫磊落险峰行",
                        PartIndex = 1,
                        RemoteURL = "http://115.28.189.40/tingguo/novel/tlbb/tlbb1.flab",
                        Duration = 5589.468,
                        Authors = new[] { new Author { Id = 1, Name = "金庸" } },
                        Narrator = "倪清",
                    },
                    new AudioEpisode {
                        Id = 2,
                        PartIndex = 451,
                        Title = "财商教育",
                        RemoteURL = "http://fruitlab.net/tinggo/tuokouxiu/dwxdl/dwxdl_451.flab",
                        Duration = 1509.222,
                        Authors = new[] {
                            new Author { Id = 3, Name = "梁冬" },
                            new Author { Id = 4, Name = "吴伯凡" }
                        },
                    },
                };
            }
        }

        public static User[] UserSamples {
            get {
                return new[] {
                    new User {
                        Id = 1,
                        Alias = "Puncsky",
                    },
                    new User {
                        Id = 2,
                        Alias = "Dobby",
                    },
                };
            }
        }

        static void AddAuthors(SQLiteConnection database) {
            database.CreateTable<Author> ();

            if (database.Table<Author> ().Count () == 0) {
                database.InsertAll (AuthorSamples);
            }
        }

        static void AddAlbum(SQLiteConnection database) {
            database.CreateTable<Album> ();

            if (database.Table<Album> ().Count () == 0) {
                database.InsertAll (AlbumSamples);
            }
        }

        static void AddEpisodes(SQLiteConnection database) {
            database.CreateTable<AudioEpisode> ();

            if (database.Table<AudioEpisode> ().Count () == 0) {
                database.InsertAll (EpisodeSamples);
            }
        }

        static void AddUsers(SQLiteConnection database) {
            database.CreateTable<User> ();

            if (database.Table<User> ().Count () == 0) {
                database.InsertAll (UserSamples);
            }
        }
    }
}

