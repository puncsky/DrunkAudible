// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using Android.Widget;
using Android.Graphics;
using Android.Content.Res;
using Android.Views;
using Android.App;
using Android.Content;

namespace DrunkAudible.Mobile.Android
{
    public static class IconProvider
    {
        const String ICON_FONT_FILENAME = "fontawesome-webfont.ttf";

        public static View CreateView (Activity context, int iconResourceStringId)
        {
            var inflater = (LayoutInflater) context.GetSystemService (Context.LayoutInflaterService);
            var tabView = inflater.Inflate (Resource.Layout.Tab, null, false);

            var tabIcon = tabView.FindViewById<TextView> (Resource.Id.TabIcon);
            tabIcon.Text = context.ApplicationContext.GetString(iconResourceStringId);
            IconProvider.ConvertTextViewToIcon (context.Assets, tabIcon);

            return tabView;
        }

        static void ConvertTextViewToIcon (AssetManager assets, TextView textViewToConvert)
        {
            Typeface font = Typeface.CreateFromAsset(assets, ICON_FONT_FILENAME);
            textViewToConvert.SetTypeface (font, TypefaceStyle.Normal);
        }
    }
}

