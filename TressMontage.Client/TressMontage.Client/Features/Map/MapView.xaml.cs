using Microsoft.Practices.Unity;
using TressMontage.Client.Core.Features.Map;
using TressMontage.Client.Features.Base;
using Xamarin.Forms;

namespace TressMontage.Client.Features.Map
{
    public abstract class MapViewBase : BindableViewBase<MapsViewModel>
    {
    }

    public partial class MapView : MapViewBase
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

        protected override MapsViewModel OnPrepareViewModel()
        {
            return App.Container.Resolve<MapsViewModel>();
        }
    }
}
