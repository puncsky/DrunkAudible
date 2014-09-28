// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using SQLite;

namespace DrunkAudible.Data.Models
{
    public class Faves
    {
        [PrimaryKey]
        [AutoIncrement]
        public int OwnerId { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }
}

