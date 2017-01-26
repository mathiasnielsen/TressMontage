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
            InitializeComponent();

            MapWebView.Source = new UrlWebViewSource
            {
                Url = "https://batchgeo.com/map/c245459265ab8747b90e21edb898ab2c",
            };
        }
    }
}
