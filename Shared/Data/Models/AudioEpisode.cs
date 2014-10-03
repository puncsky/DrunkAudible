// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using SQLite;

namespace DrunkAudible.Data.Models
{
    public class AudioEpisode : IManMadeItem, IIconAndTitleItem
    {
        static readonly AudioEpisode _empty = new AudioEpisode ();

        [Ignore]
        public Author[] Authors { get; set; }

        public double CurrentTime { get; set; } // in seconds

        public String Description { get; set; }

        public double Duration { get; set; } // in seconds

        public bool IsPurchased { get; set; }

        public string IconUrl { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public String Narrator { get; set; }

        public int PartIndex { get; set; }

        public double Price { get; set; }

        public String Title { get; set; }

        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public String RemoteURL { get; set; }

        public int FileSize { get; set; }

        [Ignore]
        public static AudioEpisode Empty { get { return _empty; } }

        public static bool IsNullOrEmpty (AudioEpisode episode)
        {
            return episode == null || episode == Empty;
        }
    }
}

