using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TressMontage.Client.Core.Http;
using TressMontage.Client.Core.Http.Clients;
using TressMontage.Entities;
using TressMontage.Utilities;

namespace TressMontage.Client.Core.Tests.Http.Clients
{
    [TestClass]
    public class TressMontageApiTests
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
            var fileName = fileNames.FirstOrDefault();

            var file = await api.GetFileAsync(fileName.DirectoryWithName, fileName.ExtensionNoDot);

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
            var firstPDF = fileNames.FirstOrDefault(x => x.Extension.Contains(".pdf"));

            var file = await api.GetFileAsync(firstPDF.DirectoryWithName, firstPDF.ExtensionNoDot);

            var testFilePath = @"C:\temp\test.pdf";
            File.WriteAllBytes(testFilePath, file);

            Assert.IsNotNull(file);
        }
    }
}
