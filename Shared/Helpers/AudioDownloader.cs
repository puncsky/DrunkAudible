// 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

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

        public static async Task<int> CreateDownloadTask(
            string url,
            IProgress<DownloadBytesProgress> progessReporter
        )
        {
            var receivedBytes = 0;
            var client = new WebClient ();
            var fileName = Uri.EscapeDataString (url);

            _throttle.WaitOne ();

            using (var storage = OpenStorage (fileName))
            using (var stream = await client.OpenReadTaskAsync (url))
            {
                var buffer = new byte[BUFFER_SIZE];
                var totalBytes = Int32.Parse (client.ResponseHeaders [HttpResponseHeader.ContentLength]);

                while (true)
                {
                    var bytesRead = await stream.ReadAsync (buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }

                    Decrypt (buffer, bytesRead);

                    storage.Write (buffer, 0, bytesRead);

                    receivedBytes += bytesRead;
                    if (progessReporter != null)
                    {
                        var args = new DownloadBytesProgress (url, receivedBytes, totalBytes);
                        progessReporter.Report (args);
                    }
                }
            }

            return receivedBytes;
        }

        public static bool HasLocalFile (String url)
        {
            var fileName = Uri.EscapeDataString (url);
            return File.Exists (GetFilePath (fileName));
        }

        static Stream OpenStorage (String fileName)
        {
            return File.Create (GetFilePath (fileName));
        }

        static String GetFilePath (String fileName)
        {
            var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
            return Path.Combine (documentsPath, fileName);
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
