// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using DrunkAudible.Data.Models;
using System.Collections.Generic;
using Android.Graphics;
using System.Threading.Tasks;

namespace DrunkAudible.Mobile.Android
{
    public class IconAndTitleItemListAdapter : BaseAdapter
    {
        readonly IIconAndTitleItem [] _items;

        public IconAndTitleItemListAdapter (Context context, IIconAndTitleItem [] items)
        {
            _items = items;
            Context = context;
        }

        public override int Count
        {
            get
            {
                return _items.Length;
            }
        }

        public override Java.Lang.Object GetItem (int position)
        {
            return null;
        }

        public override long GetItemId (int position)
        {
            return position;
        }

        public override View GetView (int position, View convertView, ViewGroup parent)
        {
            var item = _items [position];

            var inflater = (LayoutInflater) Context.GetSystemService (Context.LayoutInflaterService);
            var rowView = inflater.Inflate (Resource.Layout.AudioListViewElement, parent, false);

            var title = rowView.FindViewById<TextView> (Resource.Id.Title);
            var icon = rowView.FindViewById<ImageView> (Resource.Id.Icon);

            title.Text = item.Title;

            if (_images.ContainsKey (item.Id))
            {
                icon.SetImageBitmap (_images [item.Id]);
            }
            else
            {
                // Set the default icon. Otherwise, it may be the cached icon from the view holder.
                icon.SetImageBitmap (BitmapFactory.DecodeResource (Context.Resources, Resource.Drawable.ic_launcher));

                var listView = (AudioListView) parent;
                if (listView.ScrollState == ScrollState.Idle && !String.IsNullOrEmpty (item.IconUrl))
                {
                    StartImageDownload (listView, position, item);
                }
            }

            return rowView;
        }

        public Context Context { get; private set; }

        #region Image Support

        readonly ImageDownloader _imageDownloader = new AndroidImageDownloader ();
        readonly Dictionary<int, Bitmap> _images = new Dictionary<int, Bitmap> ();
        readonly List<int> _imageDownloadsInProgress = new List<int> ();

        public void LoadImagesForOnscreenRows (ListView listView)
        {
            for (var position = listView.FirstVisiblePosition; position <= listView.LastVisiblePosition; position++)
            {
                var audio = _items [position];
                if (!String.IsNullOrEmpty (audio.IconUrl) && !_images.ContainsKey (audio.Id))
                {
                    StartImageDownload (listView, position, audio);
                }
            }
        }

        void StartImageDownload (ListView listView, int position, IIconAndTitleItem audio)
        {
            if (_imageDownloadsInProgress.Contains (audio.Id))
            {
                return;
            }

            var url = new Uri (audio.IconUrl);

            if (_imageDownloader.HasLocallyCachedCopy (url))
            {
                var image = _imageDownloader.GetImage (url);
                FinishImageDownload (listView, position, audio, (Bitmap) image);
            }
            else
            {
                _imageDownloadsInProgress.Add (audio.Id);

                _imageDownloader.GetImageAsync (url).ContinueWith (t =>
                    {
                        if (!t.IsFaulted)
                        {
                            FinishImageDownload (listView, position, audio, (Bitmap) t.Result);
                        }
                    },
                    TaskScheduler.FromCurrentSynchronizationContext ()
                );
            }
        }

        void FinishImageDownload (ListView listView, int position, IIconAndTitleItem audio, Bitmap image)
        {
            _images [audio.Id] = image;
            _imageDownloadsInProgress.Remove (audio.Id);

            // Locate the the child view and update, because GetChildViewAt(n) only get the n-th view from the visible
            // area but position only corresponds to the _items array.
            var firstPostion = listView.FirstVisiblePosition - listView.HeaderViewsCount;
            var childIndex = position - firstPostion;
            if (0 <= childIndex && childIndex < listView.ChildCount)
            {
                var view = listView.GetChildAt (childIndex);
                var imageView = view.FindViewById<ImageView> (Resource.Id.Icon);
                imageView.SetImageBitmap (image);
            }
        }

        #endregion
    }
}

