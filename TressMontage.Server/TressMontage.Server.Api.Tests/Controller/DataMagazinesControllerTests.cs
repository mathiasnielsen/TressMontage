using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TressMontage.Server.Api.Controllers;

namespace TressMontage.Server.Api.Controller.Tests
{
    [TestClass]
    public class DataMagazinesControllerTests
    {
        [TestMethod]
        public async Task DataManazinesController_PostBlob_PostSucceded()
        {
            var controller = new DataMagazinesController();

            var dataMagazineAsText = "12345678";
            var dataMagazineAsByteArray = Convert.FromBase64String(dataMagazineAsText);
            var test = Convert.ToBase64String(dataMagazineAsByteArray);
            var test02 = Convert.ToBase64String(dataMagazineAsByteArray, 0, dataMagazineAsByteArray.Length);

            var blobPath = "Test Path.txt";

            var postResult = await controller.PostDataMagazineAsync(dataMagazineAsByteArray, blobPath);

            var postedMagazine = await controller.GetDataMagazineAsync(blobPath);
            var postedMagazineAsText = Convert.ToBase64String(postedMagazine, 0, postedMagazine.Length);

            Assert.IsTrue(postResult);
            Assert.IsTrue(dataMagazineAsText == postedMagazineAsText);
        }

        [TestMethod]
        public async Task DataManazinesController_GetBlobUrls_NotNull()
        {
            var controller = new DataMagazinesController();

            var result = await controller.GetDataMagazinesNamesAsync();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any() == true);
        }

        [TestMethod]
        public async Task DataManazinesController_GetFiles_NotNull()
        {
            var controller = new DataMagazinesController();

            var blobNames = await controller.GetDataMagazinesNamesAsync();
            var  files = new List<byte[]>();
            foreach (var blobName in blobNames)
            {
                var fileNameAsBytes = Encoding.UTF8.GetBytes(blobName);
                var encodedFileName = Convert.ToBase64String(fileNameAsBytes);

                var file = await controller.GetDataMagazineAsync(encodedFileName);
                files.Add(file);
            }

            foreach (var file in files)
            {
                Assert.IsNotNull(file);
            }
        }
    }
}
