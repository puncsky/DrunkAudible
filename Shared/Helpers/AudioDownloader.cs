// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace DrunkAudible.Mobile
{
    public class AudioDownloader
    {
        const int BUFFER_SIZE = 4096;
        const byte XOR_MAGIC_KEY = 0xAA;
        const int CONCURRENT_CONNECTION_MAX = 2;

        static readonly Semaphore _throttle = new Semaphore (CONCURRENT_CONNECTION_MAX, CONCURRENT_CONNECTION_MAX);

        public virtual async Task<int> CreateDownloadTask(
            string url,
            IProgress<DownloadBytesProgress> progressReporter
        )
        {
            var receivedBytes = 0;
            var client = new WebClient ();

            OnStartWait ();
            using (var storage = OpenStorage (url))
            using (var stream = await client.OpenReadTaskAsync (url))
            {
                _throttle.WaitOne ();
                OnStopWait ();

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
                        progressReporter.Report (args);
                    }
                }
            }

            return receivedBytes;
        }

        /// <summary>
        /// Override this method to do something while the downloader starts to wait for the throttle.
        /// e.g. Update the UI showing that the application is waiting for download.
        /// </summary>
        protected virtual void OnStartWait ()
        {
        }

        /// <summary>
        /// Override this method to do something while the downloader stops to wait for the throttle and starts to
        /// download.
        /// e.g. Update the UI showing that the application stops waiting and starts to download.
        /// </summary>
        protected virtual void OnStopWait ()
        {
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