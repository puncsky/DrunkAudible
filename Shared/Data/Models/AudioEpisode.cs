// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using SQLite;

namespace DrunkAudible.Data.Models
{
    public class AudioEpisode : IManMadeItem, IIconAndTitleItem
    {
        static readonly AudioEpisode _empty = new AudioEpisode () { Title = "DrunkAudible - No Title Selected!" };

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
        [Indexed]
        public int Id { get; set; }

        public String RemoteURL { get; set; }

        public int FileSize { get; set; }

        [Ignore]
        public static AudioEpisode Empty { get { return _empty; } }

        public static bool IsNullOrEmpty (AudioEpisode episode)
        {
            return episode == null || episode == Empty;
        }

        #region Object overrides

        public override bool Equals (object obj)
        {
            return this == (AudioEpisode) obj;
        }

        public override int GetHashCode ()
        {
            return base.GetHashCode () ^ Id.GetHashCode ();
        }

        public static bool operator == (AudioEpisode a, AudioEpisode b)
        {
            if (Object.ReferenceEquals (a, b))
            {
                return true;
            }

            // Comparision without conversion to object will result in infinite recursion.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Id == b.Id;
        }

        public static bool operator != (AudioEpisode a, AudioEpisode b)
        {
            return !(a == b);
        }

        #endregion
    }
}

