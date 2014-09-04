// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Collections.Generic;
using SQLite;

namespace DrunkAudible.Data.Models
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
        public IEnumerable<Album> Portfolio { get; set; }
    }
}

