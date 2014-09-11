// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Android.OS;

namespace Mobile.Android
{
    public class StreamingBackgroundServiceBinder : Binder
    {
        readonly StreamingBackgroundService _service;

        public StreamingBackgroundServiceBinder (StreamingBackgroundService service)
        {
            _service = service;
        }

        public StreamingBackgroundService Service
        {
            get
            {
                return _service;
            }
        }
    }
}

