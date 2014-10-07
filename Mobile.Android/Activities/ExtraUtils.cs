// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using Android.Content;
using DrunkAudible.Data.Models;
using System.Linq;
using DrunkAudible.Data;

namespace DrunkAudible.Mobile.Android
{
    public static class ExtraUtils
    {
        public const String ALBUM_ID_INTENT_EXTRA = "AlbumID";
        public const String EPISODE_ID_INTENT_EXTRA = "EpisodeID";
        public const String SELECTED_TAB = "SelectedTab";

        public static Album GetAlbum (DrunkAudibleMobileDatabase database, Intent intent)
        {
            var album = database
                    .Albums
                    .FirstOrDefault (a => a.Id == intent.GetIntExtra (ALBUM_ID_INTENT_EXTRA, -1));

            return album ?? Album.Empty;
        }

        public static AudioEpisode GetAudioEpisode (Intent intent, Album album)
        {
            if (album == null || album.Episodes == null)
            {
                return AudioEpisode.Empty;
            }

            var episode = album
                    .Episodes
                    .FirstOrDefault (e => e.Id == intent.GetIntExtra (EPISODE_ID_INTENT_EXTRA, -1));

            return episode ?? AudioEpisode.Empty;
        }

        public static void PutAlbum (Intent intent, int albumId)
        {
            intent.PutExtra (ALBUM_ID_INTENT_EXTRA, albumId);
        }

        public static void PutEpisode (Intent intent, int episodeId)
        {
            intent.PutExtra (EPISODE_ID_INTENT_EXTRA, episodeId);
        }

        public static void PutSelectedTab (Intent intent, int tabIndex)
        {
            intent.PutExtra (SELECTED_TAB, tabIndex);
        }

        public static int GetSelectedTab (Intent intent)
        {
            return intent.GetIntExtra (SELECTED_TAB, -1);
        }
    }
}

