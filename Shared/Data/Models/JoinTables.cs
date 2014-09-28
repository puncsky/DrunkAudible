// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using SQLite;

namespace DrunkAudible.Data.Models.JoinTables
{
    // All the Join Tables should only talk to ObjectRelationalMapping

    [Table ("JoinTable_AudioEpisodesToAuthors")]
    public class AudioEpisodesToAuthors
    {
        public int EpisodeId { get; set; }

        public int AuthorId { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }

    // Must update this
    [Table ("JoinTable_AudioEpisodesToAlbum")]
    public class AudioEpisodesToAlbum
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public int EpisodeId { get; set; }

        public int AlbumId { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }

    // Must update this
    [Table ("JoinTable_AlbumToAuthors")]
    public class AlbumToAuthors
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        public int AlbumId { get; set; }

        public int AuthorId { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }

    [Table ("JoinTable_UsersToFaveAlbum")]
    public class UsersToFaveAlbum
    {
        public int UserId { get; set; }

        public int AlbumId { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }

    [Table ("JoinTable_UsersToFaveEpisodes")]
    public class UsersToFaveEpisodes
    {
        public int UserId { get; set; }

        public int EpisodeId { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }

    [Table ("JoinTable_UsersToFaveAuthors")]
    public class UsersToFaveAuthors
    {
        public int UserId { get; set; }

        public int EpisodeId { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }
}

