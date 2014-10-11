// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Content;
using Android.Widget;
using System;
using Android.Runtime;
using Java.Interop;
using System.Net.Http;
using Android.Util;

namespace DrunkAudible.Mobile.Android
{
    public class StoreFragment : Fragment
    {
        const String DEBUG_TAG = "StoreFragment";
        const String onlineStoreUrl = "http://192.168.1.2:8080";
        const String audioServiceUrl = "http://192.168.1.2:8000";

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate (Resource.Layout.StoreWebView, container, false);
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            var webView = Activity.FindViewById<WebView> (Resource.Id.StoreWebView);
            webView.SetWebViewClient (new WebViewClient ());
            webView.Settings.JavaScriptEnabled = true;
            webView.AddJavascriptInterface (new WebAppInterface(Activity), "Android");
            webView.LoadUrl (onlineStoreUrl);
        }

        public class WebAppInterface : Java.Lang.Object
        {
            Context _context;

            public WebAppInterface(Context context)
            {
                _context = context;
            }

            public WebAppInterface (IntPtr handle, JniHandleOwnership transfer)
                : base (handle, transfer)
            {
            }

            [Export ("GetAlbum")]
            public async void GetAlbum (Java.Lang.String id)
            {
                var client = new HttpClient ();
                var response = await client.GetAsync (audioServiceUrl + "/album/" + id + "/");
                var content = await response.Content.ReadAsStringAsync ();

                Log.Debug (DEBUG_TAG, content);
            }

            [Export ("ShowToast")]
            public void ShowToast(Java.Lang.String toast)
            {
                Toast.MakeText(_context, toast, ToastLength.Short).Show();
            }
        }
    }
}

