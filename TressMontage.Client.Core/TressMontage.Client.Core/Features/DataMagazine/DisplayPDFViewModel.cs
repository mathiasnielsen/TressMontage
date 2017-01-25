using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TressMontage.Client.Core.Features.Base;

namespace TressMontage.Client.Core
{
    public class DisplayPDFViewModel : BindableViewModelBase
    {
        public const string PDFDirectoryParameterKey = nameof(PDFDirectoryParameterKey);

        private string pDFDirectory;

        public override Task OnViewInitialized(Dictionary<string, string> navigationParameters)
        {
            if (navigationParameters.ContainsKey(PDFDirectoryParameterKey))
            {
                PDFDirectory = navigationParameters[PDFDirectoryParameterKey];
            }

            return base.OnViewInitialized(navigationParameters);
        }

        public string PDFDirectory
        {
            get { return pDFDirectory; }
            set { Set(ref pDFDirectory, value); }
        }
    }
}