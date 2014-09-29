// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Collections.Generic;
using SQLite;

namespace DrunkAudible.Data.Models
{
    public class Album : IManMadeItem, IIconAndTitleItem
    {
        [Ignore]
        public Author[] Authors { get; set; }

        public String Description { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public String Narrator { get; set; }

        public string IconUrl { get; set; }

        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public String Title { get; set; }

        [Ignore]
        public AudioEpisode[] Episodes { get; set; }
    }
}

