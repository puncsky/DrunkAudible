// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using SQLite;

namespace DrunkAudible.Data.Models
{
    public class Album : IManMadeItem, IIconAndTitleItem
    {
        static readonly Album _empty = new Album ();

        [Ignore]
        public Author[] Authors { get; set; }

        public String Description { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public String Narrator { get; set; }

        public string IconUrl { get; set; }

        [PrimaryKey]
        [AutoIncrement]
        [Indexed]
        public int Id { get; set; }

        public String Title { get; set; }

        [Ignore]
        public AudioEpisode[] Episodes { get; set; }

        [Ignore]
        public static Album Empty { get { return _empty; } }

        public static bool IsNullOrEmpty (Album album)
        {
            return album == null || album == Empty;
        }

        #region Object overrides

        public override bool Equals (object obj)
        {
            return this == (Album) obj;
        }

        public override int GetHashCode ()
        {
            return base.GetHashCode () ^ Id.GetHashCode ();
        }

        public static bool operator == (Album a, Album b)
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

        public static bool operator != (Album a, Album b)
        {
            return !(a == b);
        }

        #endregion
    }
}

