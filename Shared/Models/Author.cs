using System;
using System.Collections.Generic;
using SQLite;

namespace DrunkAudible.Models
{
    public class Author
    {
        public String Description { get; set; }

        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public String Name { get; set; }

        [Ignore]
        public IEnumerable<AudioSeries> Portfolio { get; set; }
    }
}

