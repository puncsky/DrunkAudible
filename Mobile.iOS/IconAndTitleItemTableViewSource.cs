// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Linq;
using Foundation;
using UIKit;
using DrunkAudible.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;

namespace Mobile.iOS
{
    public class IconAndTitleItemTableViewSource : UITableViewSource
    {
        const String CELL_IDENTIFIER = "IconAndTitleItemTableViewSource";

        readonly IIconAndTitleItem [] _items;

        public IconAndTitleItemTableViewSource (IIconAndTitleItem[] items)
        {
            _items = items;
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell (CELL_IDENTIFIER);
            if (cell == null)
            {
                cell = new UITableViewCell (UITableViewCellStyle.Default, CELL_IDENTIFIER);
            }

            cell.Tag = indexPath.Row;

            var item = _items [indexPath.Row];
            cell.TextLabel.Text = item.Title;

            if (_images.ContainsKey (item.Id))
            {
                cell.ImageView.Frame = new RectangleF (0, 0, 5, 5);

                cell.ImageView.Image = _images [item.Id];
            }
            else
            {
                // Set the default icon.
                cell.ImageView.Frame = new RectangleF (0, 0, 5, 5);

                cell.ImageView.Image = _placeholder;


                if (!String.IsNullOrEmpty (item.IconUrl))
                {
                    StartImageDownload (tableView, indexPath, item);
                }
            }

            return cell;
        }

        public override nint RowsInSection (UITableView tableview, nint section)
        {
            return _items.Length;
        }

        #region Image Support

        static readonly UIImage _placeholder = UIImage.FromBundle ("first.png");
        readonly Dictionary<int, UIImage> _images = new Dictionary<int, UIImage> ();
        readonly List<int> _imageDownloadsInProgress = new List<int> ();

        readonly iOSImageDownloader _imageDownloader = new iOSImageDownloader ();

        void StartImageDownload (UITableView tableView, NSIndexPath indexPath, IIconAndTitleItem audio)
        {
            if (_imageDownloadsInProgress.Contains (audio.Id))
            {
                return;
            }

            var url = new Uri (audio.IconUrl);

            if (_imageDownloader.HasLocallyCachedCopy (url))
            {
                var image = _imageDownloader.GetImage (url);
                FinishImageDownload (tableView, indexPath, audio, (UIImage) image);
            }
            else
            {
                _imageDownloadsInProgress.Add (audio.Id);

                _imageDownloader.GetImageAsync (url).ContinueWith (t =>
                    {
                        if (!t.IsFaulted)
                        {
                            FinishImageDownload (tableView, indexPath, audio, (UIImage) t.Result);
                        }
                    },
                    TaskScheduler.FromCurrentSynchronizationContext ()
                );
            }
        }

        void FinishImageDownload (UITableView tableView, NSIndexPath indexPath, IIconAndTitleItem audio, UIImage image)
        {
            _images [audio.Id] = image;
            _imageDownloadsInProgress.Remove (audio.Id);

            // Locate the the visible view and update.
            var cell = tableView.VisibleCells.FirstOrDefault (c => c.Tag == indexPath.Row);
            if (cell != null)
            {
                cell.ImageView.Frame = new RectangleF (0, 0, 5, 5);

                tableView.CellAt (indexPath).ImageView.Image = image;

            }
        }

        #endregion
    }
}
