// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Android.Widget;
using Android.Content;
using Android.Util;

namespace DrunkAudible.Mobile.Android
{
    public class AudioListView : ListView
    {
        public ScrollState ScrollState { get; private set; }

        public AudioListView (Context context, IAttributeSet attrs) :
            base (context, attrs)
        {
            Initialize ();
        }

        public AudioListView (Context context, IAttributeSet attrs, int defStyle) :
            base (context, attrs, defStyle)
        {
            Initialize ();
        }

        void Initialize ()
        {
            ScrollState = ScrollState.Idle;
            ScrollStateChanged += HandleScrollStateChanged;
            FastScrollEnabled = true;
        }

        void HandleScrollStateChanged (object sender, ScrollStateChangedEventArgs e)
        {
            ScrollState = e.ScrollState;

            if (e.ScrollState == ScrollState.Idle)
            {
                ((IconAndTitleItemListAdapter) Adapter).LoadImagesForOnscreenRows (this);
            }
        }
    }
}

