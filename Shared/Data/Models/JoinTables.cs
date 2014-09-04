// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using SQLite;

namespace DrunkAudible.Data.Models.JoinTables
{
    // All the Join Tables should only talk to ObjectRelationalMapping

    [Table ("JoinTable_AudioEpisodesToAuthors")]
    public class AudioEpisodesToAuthors
    {
        public int EpisodeID { get; set; }

        public int AuthorID { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }

    // Must update this
    [Table ("JoinTable_AudioEpisodesToAlbum")]
    public class AudioEpisodesToAlbum
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }

        public int EpisodeID { get; set; }

        public int AlbumID { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }

    // Must update this
    [Table ("JoinTable_AlbumToAuthors")]
    public class AlbumToAuthors
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ID { get; set; }

        public int AlbumID { get; set; }

        public int AuthorID { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }

    [Table ("JoinTable_UsersToFaveAlbum")]
    public class UsersToFaveAlbum
    {
        public int UserID { get; set; }

        public int AlbumID { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }

    [Table ("JoinTable_UsersToFaveEpisodes")]
    public class UsersToFaveEpisodes
    {
        public int UserID { get; set; }

        public int EpisodeID { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }

    [Table ("JoinTable_UsersToFaveAuthors")]
    public class UsersToFaveAuthors
    {
        public int UserID { get; set; }

        public int EpisodeID { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }
}

