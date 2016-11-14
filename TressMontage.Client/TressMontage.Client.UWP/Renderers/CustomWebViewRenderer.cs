using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Client.Controls;
using TressMontage.Client.UWP.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(CustomWebViewRenderer))]
namespace TressMontage.Client.UWP.Renderers
{
    public class CustomWebViewRenderer : WebViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var customWebView = Element as CustomWebView;

                customWebView.PropertyChanged += OnPropertyChanged;

                customWebView.Uri = "BookPreview2-Ch18-Rel0417.pdf";
                Control.Source = new Uri(string.Format("ms-appx-web:///Assets/pdfjs/web/viewer.html?file={0}", string.Format("ms-appx-web:///Assets/Content/{0}", WebUtility.UrlEncode(customWebView.Uri))));
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var customWebView = sender as CustomWebView;
            if (e.PropertyName == nameof(customWebView.Uri))
            {
                ////var urlEncode = WebUtility.UrlEncode(customWebView.Uri);
                ////Control.Source = new Uri(string.Format("ms-appx-web:///Assets/pdfjs/web/viewer.html?file={0}", urlEncode));
            }
        }
    }
}