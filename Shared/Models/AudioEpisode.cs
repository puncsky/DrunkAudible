using System;
using System.IO;
using SQLite;
using System.Collections.Generic;

namespace DrunkAudible.Models
{
    public class AudioEpisode
    {
        [Ignore]
        public IEnumerable<Author> Authors { get; set; }

        public double CurrentTime { get; set; }

        public String Description { get; set; }

        public double Duration { get; set; }

        public bool IsPurchased { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public String Narrator { get; set; }

        public int PartIndex { get; set; }

        public double Price { get; set; }

        [Ignore]
        public AudioSeries Series { get; set; }

        public String Title { get; set; }

        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }

        #region FileMetaInfo

        public String RemoteURL { get; set; }

        public String LocalURL { get; set; }

        [Ignore]
        public bool IsDownloaded {
            get {
                return File.Exists (LocalURL);
            }
        }

        #endregion
    }
}

