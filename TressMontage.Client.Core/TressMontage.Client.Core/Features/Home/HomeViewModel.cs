using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Client.Core.Common;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Core.Services;

namespace TressMontage.Client.Core.Features.Home
{
    public class HomeViewModel : BindableViewModelBase
    {
        private readonly INavigationService navigationService;

        public HomeViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            NavigateToMapCommand = new RelayCommand(NavigateToMap);
            NavigateToDataMagazineCommand = new RelayCommand(NavigateToDataMagazine);
        }

        public RelayCommand NavigateToDataMagazineCommand { get; }

        public RelayCommand NavigateToMapCommand { get; }

        private void NavigateToDataMagazine()
        {
            navigationService.NavigateToDataMagazine();
        }

        private void NavigateToMap()
        {
            navigationService.NavigateToMap();
        }
    }
}
