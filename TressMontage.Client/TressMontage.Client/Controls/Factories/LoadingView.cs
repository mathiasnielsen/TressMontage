using System;
using Xamarin.Forms;

namespace TressMontage.Client.Controls.Factories
{
    public class LoadingView : StackLayout
    {
        public LoadingView(View parentContainer)
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

            ProgressLabel = new Label();
            ProgressLabel.WidthRequest = 100;
            ProgressLabel.HeightRequest = 100;
            ProgressLabel.HorizontalOptions = LayoutOptions.Center;
            ProgressLabel.HorizontalTextAlignment = TextAlignment.Center;
            ProgressLabel.FontSize = 13;
            ResetProgressLabel();

            stackLayout.Children.Add(activitySpinner);
            stackLayout.Children.Add(ProgressLabel);

            VerticalOptions = LayoutOptions.FillAndExpand;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            BackgroundColor = new Color(0, 0, 0, 0.4f);

            Children.Add(stackLayout);

            var centerX = Constraint.RelativeToParent(parent => 0);
            var centerY = Constraint.RelativeToParent(parent => 0);
            var width = Constraint.RelativeToParent(parent => parent.Width);
            var height = Constraint.RelativeToParent(parent => parent.Height);

            var contentAsRelativeLayout = parentContainer as RelativeLayout;
            contentAsRelativeLayout.Children.Add(this, centerX, centerY, width, height);

            this.IsVisible = false;
        }

        public Label ProgressLabel { get; private set; }

        public void ResetProgressLabel()
        { 
            ProgressLabel.IsVisible = false;
            ProgressLabel.Text = "0";
        }
    }
}
