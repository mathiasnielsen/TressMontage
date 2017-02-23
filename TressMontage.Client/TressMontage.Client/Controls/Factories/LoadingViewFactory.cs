using System;
using Xamarin.Forms;

namespace TressMontage.Client.Controls.Factories
{
    public class LoadingViewFactory
    {
        private StackLayout loadingContainer;

        public StackLayout ConstructView(View parentContainer)
        { 
            var stackLayout = new StackLayout();
            stackLayout.Padding = new Thickness(20, 20, 20, 40);
            stackLayout.VerticalOptions = LayoutOptions.CenterAndExpand;
            stackLayout.HorizontalOptions = LayoutOptions.CenterAndExpand;

            var activitySpinner = new ActivityIndicator();
            activitySpinner.WidthRequest = 100;
            activitySpinner.HeightRequest = 100;
            activitySpinner.Color = Color.White;
            activitySpinner.IsVisible = true;
            activitySpinner.IsRunning = true;

            stackLayout.Children.Add(activitySpinner);

            loadingContainer = new StackLayout();
            loadingContainer.VerticalOptions = LayoutOptions.FillAndExpand;
            loadingContainer.HorizontalOptions = LayoutOptions.FillAndExpand;
            loadingContainer.BackgroundColor = new Color(0, 0, 0, 0.4f);

            loadingContainer.Children.Add(stackLayout);

            var centerX = Constraint.RelativeToParent(parent => 0);
            var centerY = Constraint.RelativeToParent(parent => 0);
            var width = Constraint.RelativeToParent(parent => parent.Width);
            var height = Constraint.RelativeToParent(parent => parent.Height);

            var contentAsRelativeLayout = parentContainer as RelativeLayout;
            contentAsRelativeLayout.Children.Add(loadingContainer, centerX, centerY, width, height);

            loadingContainer.IsVisible = false;

            return loadingContainer;
        }
    }
}
