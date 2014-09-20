// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Android.Widget;
using System.Collections.Generic;
using Android.Views;
using Android.Util;
using DrunkAudible.Mobile.Android;

namespace DrunkAudible.Mobile
{
    public class AudioDownloader
    {
        const int BUFFER_SIZE = 4096;
        const byte XOR_MAGIC_KEY = 0xAA;
        const int CONCURRENT_CONNECTION_MAX = 2;
        const String DEBUG_TAG = "AudioDownloader";

        static readonly Semaphore _throttle = new Semaphore (CONCURRENT_CONNECTION_MAX, CONCURRENT_CONNECTION_MAX);
        static readonly Dictionary<String, View> _audioViewsDownloadInProgress = new Dictionary<String, View> ();

        // TODO decouple for Android-and-iOS shared abstraction.
        public static async Task<int> CreateDownloadTask(
            string url,
            ProgressBar progressBar
        )
        {
            var progressReporter = new Progress<DownloadBytesProgress> ();
            progressReporter.ProgressChanged += (s, args) =>
            {
                progressBar.Progress = (int) (progressBar.Max * args.PercentComplete);
            };

            var receivedBytes = 0;
            var client = new WebClient ();

            progressBar.Indeterminate = true;
            using (var storage = OpenStorage (url))
            using (var stream = await client.OpenReadTaskAsync (url))
            {
                _throttle.WaitOne ();
                progressBar.Indeterminate = false;

                var buffer = new byte[BUFFER_SIZE];
                var totalBytes = Int32.Parse (client.ResponseHeaders [HttpResponseHeader.ContentLength]);

                while (true)
                {
                    var bytesRead = await stream.ReadAsync (buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        _throttle.Release ();
                        break;
                    }

                    Decrypt (buffer, bytesRead);

                    storage.Write (buffer, 0, bytesRead);

                    receivedBytes += bytesRead;
                    if (progressReporter != null)
                    {
                        var args = new DownloadBytesProgress (url, receivedBytes, totalBytes);
                        ((IProgress<DownloadBytesProgress>) progressReporter).Report (args);
                    }
                }
            }

            return receivedBytes;
        }

        public static bool HasLocalFile (String url)
        {
            return File.Exists (GetFilePath (url));
        }

        public static bool HasLocalFile (String url, long expectedSize)
        {
            if (!HasLocalFile (url))
            {
                return false;
            }

            return expectedSize == new FileInfo(GetFilePath(url)).Length;
        }

        public static String GetFilePath (String url)
        {
            var fileName = Uri.EscapeDataString (url);
            var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
            return Path.Combine (documentsPath, fileName);
        }

        public static Dictionary<String, View> AudioViewsDownloadInProgress
        { 
            get { return _audioViewsDownloadInProgress; }
        }

        public static async Task StartDownloadAsync(int position, String url, ListView listView)
        {
            if (AudioViewsDownloadInProgress.ContainsKey (url))
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

                AudioViewsDownloadInProgress.Add (url, view);

                await AudioDownloader.CreateDownloadTask (url, progressBar).ContinueWith (task =>
                    {
                        if (!task.IsFaulted)
                        {
                            AudioViewsDownloadInProgress.Remove(url);
                            Log.Debug (DEBUG_TAG, "Downloaded {0} bytes.", task.Result);
                        }
                    }
                );
            }
        }

        static Stream OpenStorage (String url)
        {
            return File.Create (GetFilePath (url));
        }

        static void Decrypt (byte[] bytes, int size)
        {
            for (var i = 0; i < size; i++)
            {
                bytes [i] ^= XOR_MAGIC_KEY;
            }
        }
    }
}

