using System;
using TressMontage.Client;
using TressMontage.Client.iOS.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomLoadingOverlay), typeof(IOSLoadingOverlay))]
namespace TressMontage.Client.iOS.Controls
{
    public class IOSLoadingOverlay : ViewRenderer<CustomLoadingOverlay, UIView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomLoadingOverlay> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                SetNativeControl(new UIView());
            }

            var customLoadingOverlay = Element as CustomLoadingOverlay;

            var rect = new CoreGraphics.CGRect()
            {
                X = 0,
                Y = 0,
                Height = 300,
                Width = 300

                ////Height = (float)customLoadingOverlay.Bounds.Height,
                ////Width = (float)customLoadingOverlay.Bounds.Width
            };

            var loadingOverlay = new LoadingOverlay(rect);
        }
    }
}
