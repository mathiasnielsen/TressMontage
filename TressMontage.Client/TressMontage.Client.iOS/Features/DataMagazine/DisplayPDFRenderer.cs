using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using Foundation;
using TressMontage.Client;
using TressMontage.Client.Features.DataMagazine;
using TressMontage.Client.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(DisplayPDFRenderer))]
namespace TressMontage.Client.iOS
{
    public class DisplayPDFRenderer : ViewRenderer<CustomWebView, UIWebView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new UIWebView());
            }
            if (e.OldElement != null)
            {
                // Cleanup
            }
            if (e.NewElement != null)
            {
                var customWebView = Element as CustomWebView;
                customWebView.PropertyChanged += OnPropertyChanged;

                string fileName = Path.Combine(NSBundle.MainBundle.BundlePath, string.Format("Content/{0}", WebUtility.UrlEncode(customWebView.Uri)));
                Control.LoadRequest(new NSUrlRequest(new NSUrl(fileName, false)));
                Control.ScalesPageToFit = true;
            }
        }

        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var customWebView = sender as CustomWebView;
            if (customWebView.Uri != null)
            {
                Control.LoadRequest(new NSUrlRequest(new NSUrl(customWebView.Uri, false)));
                Control.ScalesPageToFit = true;
            }
        }
   }
}
