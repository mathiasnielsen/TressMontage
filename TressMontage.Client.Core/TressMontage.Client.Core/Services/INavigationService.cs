using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Entities;

namespace TressMontage.Client.Core.Services
{
    public interface INavigationService
    {
        void GoBack();

        void NavigateToHome();

        void NavigateToMap();

        void NavigateToDataMagazine(Folder folder = null);

        void NavigateToPdfViewer(string pdfPath);
    }
}
