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
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace DrunkAudible.Mobile
{
    /// <summary>
    /// This class only allows a specific number of WebRequests to execute
    /// simultaneously.
    /// </summary>
    public class ThrottledHttp
    {
        readonly Semaphore _throttle;

        public ThrottledHttp (int maxConcurrent)
        {
            _throttle = new Semaphore (maxConcurrent, maxConcurrent);
        }

        /// <summary>
        /// Get the specified resource. Blocks the thread until
        /// the throttling logic allows it to execute.
        /// </summary>
        /// <param name='uri'>
        /// The URI of the resource to get.
        /// </param>
        public Stream Get (Uri uri)
        {
            _throttle.WaitOne ();

            var req = WebRequest.Create (uri);

            var getTask = Task.Factory.FromAsync<WebResponse> (req.BeginGetResponse, req.EndGetResponse, null);

            return getTask.ContinueWith (
                task =>
                {
                    _throttle.Release ();
                    var res = task.Result;
                    return res.GetResponseStream ();
                }
            ).Result;
        }
    }
}

