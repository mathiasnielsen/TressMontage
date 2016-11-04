using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TressMontage.Client.Core.Http;
using TressMontage.Client.Core.Http.Clients;

namespace TressMontage.Client.Core.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task RetriveBlobNames()
        {
            var httpFactory = new HttpClientFactory();
            var api = new TressMontageApi(httpFactory);

            var result = await api.GetFileNamesAsync();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task RetriveFiles()
        {
            var httpFactory = new HttpClientFactory();
            var api = new TressMontageApi(httpFactory);

            var result = await api.GetFilesAsync();

            Assert.IsNotNull(result);
        }
    }
}
