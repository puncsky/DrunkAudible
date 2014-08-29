using System;
using System.Collections.Generic;
using SQLite;
using DrunkAudible.Helpers;

namespace DrunkAudible.Models
{
    public class AudioSeries
    {
        [Ignore]
        public IEnumerable<Author> Authors { get; set; }

        public String Description { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public String Narrator { get; set; }

        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }

        public String Title { get; set; }
    }
}

