using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Client.Features.Base;
using Xamarin.Forms;
using Microsoft.Practices.Unity;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Core.Features.Map;

namespace TressMontage.Client.Features.Map
{
    public partial class MapView : ViewBase
    {
        public MapView()
        {
            Label header = new Label
            {
                Text = "WebView",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };

            WebView webView = new WebView
            {
                Source = new UrlWebViewSource
                {
                    Url = "https://batchgeo.com/map/c245459265ab8747b90e21edb898ab2c",
                },
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            // Accomodate iPhone status bar.
            this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

            // Build the page.
            this.Content = new StackLayout
            {
                Children =
                {
                    header,
                    webView
                }
            };
        }

        protected override BindableViewModelBase OnPrepareViewModel()
        {
            return App.Container.Resolve<MapsViewModel>();
        }
    }
}
