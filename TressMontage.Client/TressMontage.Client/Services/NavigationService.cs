using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TressMontage.Client.Core.Features.DataMagazine;
using TressMontage.Client.Core.Services;
using TressMontage.Client.Features.DataMagazine;
using TressMontage.Client.Features.Home;
using TressMontage.Client.Features.Map;
using TressMontage.Client.Features.ServiceReport;
using TressMontage.Entities;

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
            Configure(nameof(DisplayPdfView), typeof(DisplayPdfView));
        }

        public void NavigateToHome()
        {
            NavigateTo(nameof(HomeView));
        }

        public void NavigateToDataMagazine(Folder folder = null)
        {
            if (folder != null)
            {
                var parms = new Dictionary<string, string>();
                parms.Add(DataMagazineViewModel.FolderParameterKey, JsonConvert.SerializeObject(folder));

                NavigateTo(nameof(DataMagazineView), parameter: parms);
            }
            else
            {
                NavigateTo(nameof(DataMagazineView));
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

        public void NavigateToPdfViewer(string pdfPath)
        {
            var parms = new Dictionary<string, string>();
            parms.Add(DisplayPdfViewModel.PdfPathParameterKey, JsonConvert.SerializeObject(pdfPath));

            NavigateTo(nameof(DisplayPdfView), parameter: parms);
        }
    }
}
