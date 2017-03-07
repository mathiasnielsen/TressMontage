using System;
using System.ComponentModel;
using System.Net;
using TressMontage.Client.Features.DataMagazine;
using TressMontage.Client.UWP.Features.DataMagazine;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(DisplayPDFRenderer))]
namespace TressMontage.Client.UWP.Features.DataMagazine
{
    public class DisplayPDFRenderer : ViewRenderer<CustomWebView, WebView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (Element == null)
            {
                return;
            }

            if (Control == null)
            {
                SetNativeControl(new WebView());
            }

            if (e.NewElement != null)
            {
                var customWebView = Element as CustomWebView;
                customWebView.PropertyChanged += OnPropertyChanged;

                var pdfjsFormat = "ms-appx-web:///Assets/pdfjs/web/viewer.html?file={0}";
                var contentFormat = "ms-appx-web:///Assets/Content/{0}";
                Control.Source = new Uri(string.Format(pdfjsFormat, string.Format(contentFormat, WebUtility.UrlEncode(customWebView.Uri))));
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var customWebView = sender as CustomWebView;
            if (customWebView.Uri != null && Control != null)
            {
                var pdfjsFormat = "ms-appx-web:///Assets/pdfjs/web/viewer.html?file={0}";
                var filePath = customWebView.Uri;
                var path = string.Format(pdfjsFormat, filePath);

                Control.Source = new Uri(path);
            }
        }
    }
}
