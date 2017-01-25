using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TressMontage.Client.Core.Services
{
    public interface INavigationService
    {
        void GoBack();

        void NavigateToHome();

        void NavigateToMap();

        void NavigateToDataMagazine(string directory = null);

        void NavigateToDisplayPDF(string pdfDirectory);
    }
}
