using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Client.Core.Services;
using TressMontage.Client.Features.DataMagazine;
using TressMontage.Client.Features.Home;
using TressMontage.Client.Features.Map;
using TressMontage.Client.Features.ServiceReport;

namespace TressMontage.Client.Services
{
    public class NavigationService : NavigationBase, INavigationService
    {
        public NavigationService()
        {
            Configure(nameof(HomeView), typeof(HomeView));
            Configure(nameof(DataMagazineView), typeof(DataMagazineView));
            Configure(nameof(MapView), typeof(MapView));
            Configure(nameof(ServiceReportView), typeof(ServiceReportView));
        }

        public void NavigateToHome()
        {
            NavigateTo(nameof(HomeView));
        }

        public void NavigateToDataMagazine()
        {
            NavigateTo(nameof(DataMagazineView));
        }

        public void NavigateToMap()
        {
            NavigateTo(nameof(MapView));
        }

        public void NavigateToServiceReport()
        {
            NavigateTo(nameof(ServiceReportView));
        }
    }
}
