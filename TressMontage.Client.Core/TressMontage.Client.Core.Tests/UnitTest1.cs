using System;
using System.IO;
using System.Linq;
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

        [TestMethod]
        public async Task RetriveFile()
        {
            var httpFactory = new HttpClientFactory();
            var api = new TressMontageApi(httpFactory);

            var fileNames = await api.GetFileNamesAsync();
            var file = await api.GetFileAsync(fileNames.FirstOrDefault());

            var testFilePath = @"C:\temp\test.txt";
            File.WriteAllBytes(testFilePath, file);

            Assert.IsNotNull(file);
        }

        [TestMethod]
        public async Task RetriveFile_andSave()
        {
            var httpFactory = new HttpClientFactory();
            var api = new TressMontageApi(httpFactory);

            var fileNames = await api.GetFileNamesAsync();
            var file = await api.GetFileAsync(fileNames.FirstOrDefault(x => x.Contains(".pdf")));

            var testFilePath = @"C:\temp\test.pdf";
            File.WriteAllBytes(testFilePath, file);

            Assert.IsNotNull(file);
        }
    }
}
