using System.Collections.Generic;
using TressMontage.Client.Core;
using TressMontage.Client.Core.Services;
using TressMontage.Client.Features.Calendar;
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
            Configure(nameof(DisplayPDFView), typeof(DisplayPDFView));
            Configure(nameof(CalendarView), typeof(CalendarView));
        }

        public void NavigateToHome()
        {
            NavigateTo(nameof(HomeView));
        }

        public void NavigateToDataMagazine(string directory = null)
        {
            if (directory == null)
            {
                NavigateTo(nameof(DataMagazineView));
            }
            else
            { 
                var parms = new Dictionary<string, string>();
                parms.Add(DataMagazineViewModel.RelativeDirectoryParameterKey, directory);

                NavigateTo(nameof(DataMagazineView), parameter: parms);
            }
        }

        public void NavigateToMap()
        {
            NavigateTo(nameof(MapView));
        }

        public void NavigateToServiceReport()
        {
            NavigateTo(nameof(ServiceReportView));
        }

        public void NavigateToCalendar()
        {
            NavigateTo(nameof(CalendarView));
        }

        public void NavigateToDisplayPDF(string pdfDirectory)
        {
            var parms = new Dictionary<string, string>();
            parms.Add(DisplayPDFViewModel.PDFDirectoryParameterKey, pdfDirectory);

            NavigateTo(nameof(DisplayPDFView), parameter: parms);
        }
    }
}
