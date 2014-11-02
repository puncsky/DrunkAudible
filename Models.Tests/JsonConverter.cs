//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Newtonsoft.Json;
//using DrunkAudible.Data;
//using DrunkAudible.Data.Models;
//using NUnit;
//using NUnit.Framework;
//
//namespace DrunkAudible.Mobile.Tests
//{
//    [TestFixture]
//    public class JsonConverter
//    {
//        [Test]
//        public static void GenerateJson (string [] args)
//        {
//            Console.WriteLine ("Hello World!");
//
//            var filePaths = Directory.GetFiles ("/Users/tp/Downloads/info");
//
//            var episodes = new List<TingEpisode> ();
//
//            foreach (var filePath in filePaths)
//            {
//                var content = File.ReadAllText (filePath);
//                episodes.AddRange (JsonConvert.DeserializeObject<List<TingEpisode>> (content));
//            }
//
//            var orm = new DrunkAudibleMobileDatabase();
//
//            var albumNames = episodes.Where (e => e.type == 1).Select (e => {
//                var len = e.name.Length;
//                return e.name.Substring(0, len > 2 ? 2 : len);
//            });
//            var albums = new List<Album> ();
//
//            foreach (var albumName in albumNames)
//            {
//                var album = GetAlbum (episodes, albumName);
//
//                if (album.Episodes.Any ())
//                {
//                    //orm.InsertOrUpdate (album);
//                    albums.Add (album);
//                }
//            }
//
//            //            var album = GetAlbum (episodes, "笑傲江湖");
//            //            orm.InsertOrUpdate (album);
//
//
//            var xhjh = albums.FirstOrDefault (a => a.Title.Contains ("晓说"));
//            for (int i = 0; i < xhjh.Episodes.Count; i++)
//            {
//                xhjh.Episodes [i].SeqId = i;
//            }
//
//            var collectionJson = JsonConvert.SerializeObject (xhjh.Episodes);
//            xhjh.Episodes = null;
//            var xhjhJson = JsonConvert.SerializeObject (xhjh);
//            Console.WriteLine (episodes.Count ());
//        }
//
//        static Album GetAlbum(List<TingEpisode> tingEpisodes, string albumName)
//        {
//            var album = new Album ();
//            var tingAlbum = tingEpisodes.FirstOrDefault (e => e.type == 1 && e.name.Contains(albumName)); 
//            album.Id = tingAlbum.id;
//            album.Title = albumName;
//            var subTingEpisodes = tingEpisodes.Where (e => 
//                e.type == 0 && e.name.Contains (albumName)
//            );
//            album.Episodes = subTingEpisodes.Select (s => 
//                new AudioEpisode
//                {
//                    Id = s.id,
//                    Title = s.name,
//                    Description = s.desc,
//                    RemoteUrl = s.cont_url,
//                    Duration = s.dur,
//                    FileSize = s.fsz,
//                }
//            ).ToList ();
//
//            return album;
//        }
//    }
//
//
//    public class TingEpisode {
//        public int type { get; set; }
//        public String desc { get; set; }
//        public String name { get; set; }
//        public int newcnt { get; set; }
//        public String cont_url { get; set; }
//        public int id { get; set; }
//        public int fsz { get; set; }
//        public double dur { get; set; }
//    }
//}
//
