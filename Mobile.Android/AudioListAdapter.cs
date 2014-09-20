// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using System.Linq;
using DrunkAudible.Data.Models;
using System.Collections.Generic;
using Android.Graphics;
using System.Threading.Tasks;

namespace DrunkAudible.Mobile.Android
{
    public class AudioListAdapter : BaseAdapter
    {
        readonly IAudioListViewElement [] _items;
        readonly Context _context;
        readonly ImageDownloader _imageDownloader = new AndroidImageDownloader ();
        readonly Album _album;

        public AudioListAdapter (Context context, Album album)
            : this (context, album.Episodes.ToArray ())
        {
            _album = album;
        }

        public AudioListAdapter (Context context, IAudioListViewElement[] items)
        {
            _items = items;
            _context = context;
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
            View rowView = null;
            var item = _items [position];
            if (_album != null)
            {
                var episode = (AudioEpisode) item;
                if (AudioDownloader.AudioViewsDownloadInProgress.ContainsKey (episode.RemoteURL))
                {
                    rowView = AudioDownloader.AudioViewsDownloadInProgress [episode.RemoteURL];
                }
            }

            if (rowView == null)
            {
                var inflater = (LayoutInflater) _context.GetSystemService (Context.LayoutInflaterService);
                rowView = inflater.Inflate (Resource.Layout.AudioListViewElement, parent, false);
            }

            var title = rowView.FindViewById<TextView> (Resource.Id.Title);
            var by = rowView.FindViewById<TextView> (Resource.Id.By);
            var authors = rowView.FindViewById<TextView> (Resource.Id.Authors);
            var narratedBy = rowView.FindViewById<TextView> (Resource.Id.NarratedBy);
            var narrator = rowView.FindViewById<TextView> (Resource.Id.Narrator);
            var icon = rowView.FindViewById<ImageView> (Resource.Id.Icon);
            var downloadProgress = rowView.FindViewById<ProgressBar> (Resource.Id.DownloadProgress);

            title.Text = item.Title;

            authors.Text = GetAuthorsText (item);
            if (String.IsNullOrEmpty (authors.Text))
            {
                by.Text = String.Empty;
            }

            narrator.Text = GetNarratorText (item);
            if (String.IsNullOrEmpty (narrator.Text))
            {
                narratedBy.Text = String.Empty;
            }

            if (_images.ContainsKey (item.ID))
            {
                icon.SetImageBitmap (_images [item.ID]);
            }
            else
            {
                // Set the default icon. Otherwise, it may be the cached icon from the view holder.
                icon.SetImageBitmap (BitmapFactory.DecodeResource (_context.Resources, Resource.Drawable.ic_launcher));

                var listView = (AudioListView) parent;
                if (listView.ScrollState == ScrollState.Idle && !String.IsNullOrEmpty (item.IconUrl))
                {
                    StartImageDownload (listView, position, item);
                }
            }

            if (_album != null)
            {
                var episode = item as AudioEpisode;
                if (episode != null)
                {
                    downloadProgress.Progress =
                        AudioDownloader.HasLocalFile(episode.RemoteURL, episode.FileSize) ? downloadProgress.Max : 0;
                }
            }

            return rowView;
        }

        String GetAuthorsText (IAudioListViewElement audio)
        {
            var authors = audio.Authors != null ?
                String.Join (", ", audio.Authors.Select (a => a.Name)) : String.Empty;
            if (String.IsNullOrEmpty (authors) && _album != null && _album.Authors != null)
            {
                authors = String.Join (", ", _album.Authors.Select (a => a.Name));
            }

            return authors;
        }

        String GetNarratorText (IAudioListViewElement audio)
        {
            var narrator = audio.Narrator;
            if (String.IsNullOrEmpty (narrator) && _album != null)
            {
                narrator = _album.Narrator;
            }

            return narrator;
        }

        #region Image Support

        readonly Dictionary<int, Bitmap> _images = new Dictionary<int, Bitmap> ();
        readonly List<int> _imageDownloadsInProgress = new List<int> ();

        public void LoadImagesForOnscreenRows (ListView listView)
        {
            for (var position = listView.FirstVisiblePosition; position <= listView.LastVisiblePosition; position++)
            {
                var audio = _items [position];
                if (!String.IsNullOrEmpty (audio.IconUrl) && !_images.ContainsKey (audio.ID))
                {
                    StartImageDownload (listView, position, audio);
                }
            }
        }

        void StartImageDownload (ListView listView, int position, IAudioListViewElement audio)
        {
            if (_imageDownloadsInProgress.Contains (audio.ID))
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
                _imageDownloadsInProgress.Add (audio.ID);

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

        void FinishImageDownload (ListView listView, int position, IAudioListViewElement audio, Bitmap image)
        {
            _images [audio.ID] = image;
            _imageDownloadsInProgress.Remove (audio.ID);

            // Locate the the child view and update.
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

