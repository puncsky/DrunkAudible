// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;
using DrunkAudible.Data.Models;
using DrunkAudible.Data.Models.JoinTables;

namespace DrunkAudible.Data
{
    public static class OrmInitializer
    {
        #region Sample Data

        public static Album[] AlbumSamples {
            get {
                return new[] {
                    new Album {
                        ID = 1,
                        Title = "天龙八部",
                        Authors = new[] { new Author { ID = 1, Name = "金庸" } },
                        Narrator = "倪清",
                        IconUrl = "https://www.google.com/images/srpr/logo11w.png",
                        Episodes = new[] {
                            new AudioEpisode {
                                ID = 1,
                                Title = "青衫磊落险峰行",
                                PartIndex = 1,
                                RemoteURL = "http://115.28.189.40/tingguo/novel/tlbb/tlbb1.flab",
                                Duration = 5589.468,
                            },
                        },
                    },
                    new Album {
                        ID = 2,
                        Title = "冬吴相对论",
                        Authors = new[] { new Author { ID = 3, Name = "梁冬" }, new Author { ID = 4, Name = "吴伯凡" } },
                        Episodes = new[] {
                            new AudioEpisode {
                                ID = 2,
                                PartIndex = 451,
                                Title = "财商教育",
                                RemoteURL = "http://fruitlab.net/tinggo/tuokouxiu/dwxdl/dwxdl_451.flab",
                                Duration = 1509.222,
                            },
                        },
                    },
                    new Album {
                        ID = 3,
                        Title = "笑傲江湖",
                        Authors = new[] { new Author { ID = 1, Name = "金庸" } },
                    },
                };
            }
        }

        #endregion

        public static void Initialize(ObjectRelationalMapping orm) {
            orm.InsertOrUpdate (AlbumSamples);
        }
    }
}

