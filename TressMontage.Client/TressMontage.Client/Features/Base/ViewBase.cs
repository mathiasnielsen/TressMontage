using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Client.Core.Features.Base;
using Xamarin.Forms;

namespace TressMontage.Client.Features.Base
{
    public abstract class ViewBase : ContentPage
    {
        public BindableViewModelBase ViewModel { get; set; }

        private bool hasAppeared;

        protected abstract BindableViewModelBase OnPrepareViewModel();

        public ViewBase()
        {
            ViewModel = OnPrepareViewModel();
            BindingContext = ViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (hasAppeared == false)
            {
                ViewModel.ViewInitialized();
                hasAppeared = true;
            }
            else
            {
                ViewModel.ViewReloading();
            }
        }
    }
}
