//
//  Copyright 2012, Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//

// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved for the modification of the original file.

using System;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.IO;

namespace DrunkAudible.Mobile
{
    public abstract class ImageDownloader
    {
        readonly IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication ();

        readonly ThrottledHttp http;

        readonly TimeSpan cacheDuration;

        const int CACHE_DAYS_MAX = 1; // TODO: release version should set a larger number.

        protected ImageDownloader (int maxConcurrentDownloads = 2)
            : this (TimeSpan.FromDays (CACHE_DAYS_MAX))
        {
            http = new ThrottledHttp (maxConcurrentDownloads);
        }

        protected ImageDownloader (TimeSpan cacheDuration)
        {
            this.cacheDuration = cacheDuration;

            if (!store.DirectoryExists ("ImageCache")) {
                store.CreateDirectory ("ImageCache");
            }
        }

        public bool HasLocallyCachedCopy (Uri uri)
        {
            var now = DateTime.UtcNow;

            var filename = Uri.EscapeDataString (uri.AbsoluteUri);

            var lastWriteTime = GetLastWriteTimeUtc (filename);

            return lastWriteTime.HasValue && (now - lastWriteTime.Value) < cacheDuration;
        }

        public Task<object> GetImageAsync (Uri uri)
        {
            return Task.Factory.StartNew (() =>
                {
                    return GetImage (uri);
                });
        }

        public object GetImage (Uri uri)
        {
            var filename = Uri.EscapeDataString (uri.AbsoluteUri);

            if (HasLocallyCachedCopy (uri))
            {
                using (var o = OpenStorage (filename, FileMode.Open))
                {
                    return LoadImage (o);
                }
            }

            using (var d = http.Get (uri))
            {
                using (var o = OpenStorage (filename, FileMode.Create))
                {
                    d.CopyTo (o);
                }
            }
            using (var o = OpenStorage (filename, FileMode.Open))
            {
                return LoadImage (o);
            }
        }

        protected virtual DateTime? GetLastWriteTimeUtc (string fileName)
        {
            var path = Path.Combine ("ImageCache", fileName);
            if (store.FileExists (path)) {
                return store.GetLastWriteTime (path).UtcDateTime;
            }
            return null;
        }

        protected virtual Stream OpenStorage (string fileName, FileMode mode)
        {
            return store.OpenFile (Path.Combine ("ImageCache", fileName), mode);
        }

        protected abstract object LoadImage (Stream stream);
    }
}

