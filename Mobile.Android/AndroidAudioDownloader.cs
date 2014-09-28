// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Util;
using Android.Views;
using Android.Widget;
using DrunkAudible.Mobile;
using DrunkAudible.Mobile.Android;

namespace DrunkAudible.Mobile.Android
{
    public class AndroidAudioDownloader : AudioDownloader
    {
        const String DEBUG_TAG = "AndroidAudioDownloader";

        static readonly Dictionary<String, View> _audioViewsDownloadInProgress = new Dictionary<String, View> ();

        ProgressBar _progressBar;

        public async Task<int> CreateDownloadTask(
            string url,
            ProgressBar progressBar
        )
        {
            _progressBar = progressBar;
            var progressReporter = new Progress<DownloadBytesProgress> ();
            progressReporter.ProgressChanged += (s, args) =>
            {
                progressBar.Progress = (int) (progressBar.Max * args.PercentComplete);
            };

            return await CreateDownloadTask (url, progressReporter);
        }

        protected override void OnStartWait ()
        {
            _progressBar.Indeterminate = true;
        }

        protected override void OnStopWait ()
        {
            _progressBar.Indeterminate = false;
        }

        /// <summary>
        /// A dictionary to keep views downloading in progress by the key of Audio Id, because we want to keep the UI
        /// showing that the episode is downloading.
        /// </summary>
        /// <value>The audio views download in progress.</value>
        public static Dictionary<String, View> ViewsDownloadInProgressByAudioId
        { 
            get { return _audioViewsDownloadInProgress; }
        }

        public static async Task StartDownloadAsync(int position, String url, ListView listView)
        {
            if (ViewsDownloadInProgressByAudioId.ContainsKey (url))
            {
                return;
            }

            // Locate the the child view and update.
            var firstPostion = listView.FirstVisiblePosition - listView.HeaderViewsCount;
            var childIndex = position - firstPostion;
            if (0 <= childIndex && childIndex < listView.ChildCount)
            {
                var view = listView.GetChildAt (childIndex);
                var progressBar = view.FindViewById<ProgressBar> (Resource.Id.DownloadProgress);

                ViewsDownloadInProgressByAudioId.Add (url, view);

                var downloader = new AndroidAudioDownloader ();
                await downloader.CreateDownloadTask (url, progressBar).ContinueWith (task =>
                    {
                        if (!task.IsFaulted)
                        {
                            ViewsDownloadInProgressByAudioId.Remove(url);
                            Log.Debug (DEBUG_TAG, "Downloaded {0} bytes.", task.Result);
                        }
                    }
                );
            }
        }
    }
}

