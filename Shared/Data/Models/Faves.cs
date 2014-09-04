// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Collections.Generic;
using SQLite;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using DrunkAudible.Helpers;

namespace DrunkAudible.Data.Models
{
    public class Faves
    {
        [PrimaryKey]
        [AutoIncrement]
        public int OwnerID { get; set; }

        public DateTime LastUpdateTime { get; set; }


    }
}

