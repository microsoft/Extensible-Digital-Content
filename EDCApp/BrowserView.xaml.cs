//--------------------------------------------------------------------------------------
// BrowserView.xaml.cs
//
// Advanced Technology Group (ATG)
// Copyright (C) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;

namespace EDCApp
{
    /// <summary>
    /// The BrowserView class will attempt to display the content using a browser.
    /// </summary>
    public sealed partial class BrowserView : Page
    {
        public BrowserView()
        {
            this.InitializeComponent();
            this.DataContext = this;

            WebView.CoreWebView2Initialized += (sender, e) =>
            {
                WebView.CoreWebView2.SetVirtualHostNameToFolderMapping("DigitalContent", "DigitalContent", CoreWebView2HostResourceAccessKind.Allow);

                WebView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                WebView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

                WebView.CoreWebView2.Settings.HiddenPdfToolbarItems =
                        CoreWebView2PdfToolbarItems.Bookmarks
                        | CoreWebView2PdfToolbarItems.FitPage
                        | CoreWebView2PdfToolbarItems.PageLayout
                        | CoreWebView2PdfToolbarItems.PageSelector
                        | CoreWebView2PdfToolbarItems.Print
                        | CoreWebView2PdfToolbarItems.Rotate
                        | CoreWebView2PdfToolbarItems.Save
                        | CoreWebView2PdfToolbarItems.SaveAs
                        | CoreWebView2PdfToolbarItems.Search
                        | CoreWebView2PdfToolbarItems.ZoomIn
                        | CoreWebView2PdfToolbarItems.ZoomOut;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Content content = (Content)e.Parameter;
            WebView.Source = new Uri("http://" + content.Path);
            SharedUIViewModel.Instance.CurrentViewTitle = content.Name;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }
    }
}

