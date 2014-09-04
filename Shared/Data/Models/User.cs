// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using SQLite;
using System.Collections.Generic;

namespace DrunkAudible.Data.Models
{
    public class User
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }

        [Unique]
        [NotNull]
        public String Alias { get; set; }

        public String Description { get; set; }

        public String FirstName { get; set; }

        public String LastName { get; set; }

        public String Email { get; set; }

        public String Token { get; set; }

        public String Phone {get;set;}

        public String Address { get; set; }

        public String Address2 { get; set; }

        public String City { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public String State { get; set; }

        public String Country { get; set; }

        public String ZipCode { get; set; }

        [Ignore]
        public IEnumerable<Album> AlbumFaves { get; set; }

        [Ignore]
        public IEnumerable<AudioEpisode> EpisodeFaves { get; set; }

        [Ignore]
        public IEnumerable<Author> AuthorFaves { get; set; }
    }
}

