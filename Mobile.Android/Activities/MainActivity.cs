// (c) 2012-2014 Tian Pan (www.puncsky.com). All Rights Reserved.

using System;
using Android.App;
using Android.OS;

namespace DrunkAudible.Mobile.Android
{
    [Activity (
        Label = "@string/ApplicationName",
        MainLauncher = true,
        Icon = "@drawable/ic_launcher",
        Theme = "@style/Theme"
    )]
    public class MainActivity : Activity
    {
        const String SELECTED_TAB_BUNDLE_KEY = "Tab";

        static TabHolder [] _tabConfiguration =
        {
            new TabHolder (TabTitle.Home, Resource.String.ic_fa_home, new AlbumListFragment ()),
            new TabHolder (TabTitle.Player, Resource.String.ic_fa_play_circle, new PlayerPresenterFragment ()),
            new TabHolder (TabTitle.Store, Resource.String.ic_fa_shopping_cart, new Fragment ()),
            new TabHolder (TabTitle.Me, Resource.String.ic_fa_user, new Fragment ()),
        };

        protected override void OnCreate (Bundle savedInstanceState)
        {
            base.OnCreate (savedInstanceState);

            SetContentView (Resource.Layout.Main);

            ConfigureTabs ();

            // Restore the selected tab. e.g. rotation.
            if (savedInstanceState != null)
            {
                ActionBar.SelectTab (ActionBar.GetTabAt (savedInstanceState.GetInt (SELECTED_TAB_BUNDLE_KEY)));
            }
        }

        public ActionBar.Tab GetTab (TabTitle title)
        {
            return ActionBar.GetTabAt ((int) title);
        }

        public enum TabTitle { Home, Player, Store, Me };

        protected override void OnSaveInstanceState (Bundle outState)
        {
            base.OnSaveInstanceState (outState);

            outState.PutInt (SELECTED_TAB_BUNDLE_KEY, ActionBar.SelectedNavigationIndex);
        }

        void ConfigureTabs ()
        {
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            ActionBar.SetDisplayShowTitleEnabled (false);
            ActionBar.SetDisplayShowHomeEnabled (false);

            foreach (var tabHolder in _tabConfiguration)
            {
                AddTab (tabHolder);
            }

            var selectedTabIndex = ExtraUtils.GetSelectedTab (Intent);
            if (selectedTabIndex != -1 && ActionBar.SelectedTab != GetTab (TabTitle.Player))
            {
                Intent.RemoveExtra (ExtraUtils.SELECTED_TAB);
                ActionBar.GetTabAt (selectedTabIndex).Select ();
            }
        }

        void AddTab (TabHolder tabHolder)
        {
            var tab = ActionBar.NewTab ();
            tab.SetCustomView (IconProvider.CreateView (this, tabHolder.IconResource));

            tab.TabSelected += (sender, e) =>
            {
                var currentFragment = FragmentManager.FindFragmentById (Resource.Id.FragmentContainer);
                if (currentFragment != null)
                {
                    e.FragmentTransaction.Remove (currentFragment);
                }

                e.FragmentTransaction.Add (Resource.Id.FragmentContainer, tabHolder.Fragment);
            };
            tab.TabUnselected += (sender, e) => e.FragmentTransaction.Remove (tabHolder.Fragment);

            ActionBar.AddTab (tab);
        }

        class TabHolder
        {
            public TabHolder (TabTitle title, int iconResource, Fragment fragment)
            {
                Title = title;
                IconResource = iconResource;
                Fragment = fragment;
            }

            public TabTitle Title { get; set; }
            public int IconResource { get; set; }
            public Fragment Fragment { get; set; }
        }
    }
}

