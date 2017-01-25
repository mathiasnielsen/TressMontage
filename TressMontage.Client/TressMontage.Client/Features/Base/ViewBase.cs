using Xamarin.Forms;

namespace TressMontage.Client.Features.Base
{
    public abstract class ViewBase : ContentPage
    {
        public ViewBase()
        {
            BackgroundColor = (Color)Application.Current.Resources["ContentBackgroundColor"];
        }
    }
}
