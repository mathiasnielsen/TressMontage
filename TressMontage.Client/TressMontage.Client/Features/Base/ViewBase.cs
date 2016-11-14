using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Services;
using Xamarin.Forms;

namespace TressMontage.Client.Features.Base
{
    public abstract class ViewBase : ContentPage
    {
        public BindableViewModelBase ViewModel { get; set; }

        private bool hasAppeared;

        private Dictionary<string, string> parameters;

        protected abstract BindableViewModelBase OnPrepareViewModel();

        public ViewBase()
        {
            ViewModel = OnPrepareViewModel();
            BindingContext = ViewModel;

            BackgroundColor = (Color)App.Current.Resources["ContentBackgroundColor"];
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var args = this.GetNavigationArgs();

            if (hasAppeared == false)
            {
                ViewModel.ViewInitialized(args);
                hasAppeared = true;
            }
            else
            {
                ViewModel.ViewReloading();
            }
        }
    }
}
