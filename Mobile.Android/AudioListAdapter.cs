// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using Android.Widget;
using Android.Content;
using Android.Views;
using System.Linq;
using DrunkAudible.Data.Models;

namespace Mobile.Android
{
    public class AudioListAdapter : BaseAdapter
    {
        readonly IAudioListViewElement[] _audios;
        readonly Context _context;

        public AudioListAdapter(Context context, IAudioListViewElement[] audios)
        {
            _audios = audios;
            _context = context;
        }

        public override int Count
        {
            get
            {
                return _audios.Length;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int i)
        {
            return i;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View rowView;
            if (convertView != null)
            {
                rowView = convertView;
            }
            else
            {
                LayoutInflater inflater = (LayoutInflater)_context.GetSystemService (Context.LayoutInflaterService);
                rowView = inflater.Inflate (Resource.Layout.Main, parent, false);

                TextView nameText = (TextView)rowView.FindViewById (Resource.Id.Title);
                TextView authorsText = (TextView)rowView.FindViewById (Resource.Id.Authors);
                TextView narratedByText = (TextView)rowView.FindViewById (Resource.Id.NarratedBy);
                TextView narratorText = (TextView)rowView.FindViewById (Resource.Id.Narrator);

                ViewHolder holder = new ViewHolder ();
                holder.Title = nameText;
                holder.Authors = authorsText;
                holder.Narrator = narratorText;
                holder.NarratedBy = narratedByText;

                rowView.Tag = holder;
            }

            ViewHolder tag = (ViewHolder)rowView.Tag;
            tag.Title.Text = _audios [position].Title;
            tag.Authors.Text = String.Join (", ", _audios [position].Authors.Select (a => a.Name));
            var narrator = _audios [position].Narrator;
            if (String.IsNullOrEmpty (narrator))
            {
                tag.NarratedBy.Text = String.Empty;
            }
            tag.Narrator.Text = narrator;

            return rowView;
        }

        class ViewHolder : Java.Lang.Object
        {
            // TODO public ImageView Icon;
            public TextView Title;
            public TextView Authors;
            public TextView NarratedBy;
            public TextView Narrator;
        }
    }
}

