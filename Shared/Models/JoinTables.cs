using System;
using SQLite;

namespace DrunkAudible.Models.JoinTables
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
    [Table ("JoinTable_AudioEpisodesToSeries")]
    public class AudioEpisodesToSeries
    {
        public int EpisodeID { get; set; }

        public int SeriesID { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }

    // Must update this
    [Table ("JoinTable_AudioSeriesToAuthors")]
    public class AudioSeriesToAuthors
    {
        public int SeriesID { get; set; }

        public int AuthorID { get; set; }

        public DateTime LastUpdateTime { get; set; }
    }

    [Table ("JoinTable_UsersToFaveSeries")]
    public class UsersToFaveSeries
    {
        public int UserID { get; set; }

        public int SeriesID { get; set; }

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

