using System;
using System.Collections.Generic;
using DrunkAudible.Models;
using DrunkAudible.Models.JoinTables;
using SQLite;

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

        public static AudioSeries[] SeriesSamples {
            get {
                return new[] {
                    new AudioSeries {
                        ID = 1,
                        Title = "天龙八部",
                    },
                    new AudioSeries {
                        ID = 2,
                        Title = "冬吴相对论",
                    },
                    new AudioSeries {
                        ID = 3,
                        Title = "笑傲江湖",
                    },
                };
            }
        }

        public static AudioEpisode[] EpisodeSamples {
            get {
                return new[] {
                    new AudioEpisode {
                        Title = "青衫磊落险峰行",
                        PartIndex = 1,
                        RemoteURL = "http://115.28.189.40/tingguo/novel/tlbb/tlbb1.flab",
                        Duration = 5589.468,
                    },
                    new AudioEpisode {
                        PartIndex = 451,
                        Title = "财商教育",
                        RemoteURL = "http://fruitlab.net/tinggo/tuokouxiu/dwxdl/dwxdl_451.flab",
                        Duration = 1509.222
                    },
                };
            }
        }

        public static User[] SampleUsers {
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

        public static AudioEpisodesToSeries[] SampleAudioEpisodesToSeries {
            get {
                return new[] {
                    new AudioEpisodesToSeries {
                        EpisodeID = 1,
                        SeriesID = 1,
                    },
                    new AudioEpisodesToSeries {
                        EpisodeID = 2,
                        SeriesID = 2,
                    },
                };
            }
        }

        public static AudioSeriesToAuthors[] SampleAudioSeriesToAuthors {
            get {
                return new[] {
                    new AudioSeriesToAuthors {
                        SeriesID = 1,
                        AuthorID = 1,
                    },
                    new AudioSeriesToAuthors {
                        SeriesID = 2,
                        AuthorID = 3,
                    },
                    new AudioSeriesToAuthors {
                        SeriesID = 2,
                        AuthorID = 4,
                    },
                    new AudioSeriesToAuthors {
                        SeriesID = 3,
                        AuthorID = 1,
                    },
                };
            }
        }

        #endregion

        public static void Initialize(SQLiteConnection database) {
            AddAuthors (database);
            AddSeries (database);
            AddEpisodes (database);
        }

        private static void AddAuthors(SQLiteConnection database) {
            database.CreateTable<Author> ();

            if (database.Table<Author> ().Count () == 0) {
                database.InsertAll (AuthorSamples);
            }
        }

        private static void AddSeries(SQLiteConnection database) {
            database.CreateTable<AudioSeries> ();

            if (database.Table<AudioSeries> ().Count () == 0) {
                database.InsertAll (SeriesSamples);
            }
        }

        private static void AddEpisodes(SQLiteConnection database) {
            database.CreateTable<AudioEpisode> ();

            if (database.Table<AudioEpisode> ().Count () == 0) {
                database.InsertAll (EpisodeSamples);
            }
        }
    }
}

