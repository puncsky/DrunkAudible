// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Android.OS;

namespace DrunkAudible.Mobile.Android
{
    public class PlayerServiceBinder : Binder
    {
        readonly PlayerService _service;

        public PlayerServiceBinder (PlayerService service)
        {
            _service = service;
        }

        public PlayerService Service
        {
            get
            {
                return _service;
            }
        }
    }
}

