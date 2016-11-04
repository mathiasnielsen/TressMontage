using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TressMontage.Client.Core.Http.Clients
{
    public class TressMontageApi : ITressMontageApi
    {
        private readonly IHttpRequestExecutor executor;

        public TressMontageApi(IHttpClientFactory httpClientFactory)
        {
            executor = new HttpRequestExecutor(httpClientFactory);
        }
    }
}
