// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using System.Linq;
using Android.Views;
using Android.Widget;
using DrunkAudible.Data.Models;

namespace DrunkAudible.Mobile.Android
{
    public static class ViewExtensions
    {
        public static void SetNarratorsAndAuthors(this View iconAndTitleView, IManMadeItem item)
        {
            var by = iconAndTitleView.FindViewById<TextView> (Resource.Id.By);
            var authors = iconAndTitleView.FindViewById<TextView> (Resource.Id.Authors);
            var narratedBy = iconAndTitleView.FindViewById<TextView> (Resource.Id.NarratedBy);
            var narrator = iconAndTitleView.FindViewById<TextView> (Resource.Id.Narrator);

            authors.Text = GetAuthorsText (item);
            if (String.IsNullOrEmpty (authors.Text))
            {
                by.Text = String.Empty;
            }

            narrator.Text = item.Narrator;
            if (String.IsNullOrEmpty (narrator.Text))
            {
                narratedBy.Text = String.Empty;
            }
        }

        static String GetAuthorsText (IManMadeItem item)
        {
            return item.Authors == null ? String.Empty : String.Join (", ", item.Authors.Select (a => a.Name));
        }
    }
}

