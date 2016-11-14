using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TressMontage.Client.Core.Features.Base;

namespace TressMontage.Client.Core.Features.DataMagazine
{
    public class DisplayPdfViewModel : BindableViewModelBase
    {
        public const string PdfPathParameterKey = nameof(PdfPathParameterKey);

        private string pdfPath;

        public string PdfPath
        {
            get { return pdfPath; }
            set { Set(ref pdfPath, value); }
        }

        public override Task OnViewInitialized(Dictionary<string, string> parameters)
        {
            if (parameters.Keys.Contains(PdfPathParameterKey))
            {
                PdfPath = JsonConvert.DeserializeObject<string>(parameters[PdfPathParameterKey]);
            }

            return base.OnViewInitialized(parameters);
        }
    }
}
