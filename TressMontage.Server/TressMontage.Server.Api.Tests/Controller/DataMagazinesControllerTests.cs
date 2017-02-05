using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TressMontage.Entities;
using TressMontage.Server.Api.Controllers;
using TressMontage.Utilities;

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

            var fileInfo = new FileInfo()
            {
                Name = "test blob path",
                Path = "testPath",
                Type = FileTypes.TXT
            };

            var fileDto = new FileDTO()
            {
                Data = dataMagazineAsByteArray,
                FileInfo = fileInfo
            };

            var postResult = await controller.PostDataMagazineAsync(fileDto);

            var postedMagazine = await controller.GetDataMagazineAsync(fileInfo.Name);
            var postedMagazineAsText = Convert.ToBase64String(postedMagazine, 0, postedMagazine.Length);

            Assert.IsTrue(postResult);
            Assert.IsTrue(dataMagazineAsText == postedMagazineAsText);
        }

        [TestMethod]
        public void DataManazinesController_GetBlobUrls_NotNull()
        {
            var controller = new DataMagazinesController();

            var result = controller.GetDataMagazinesInfo();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any() == true);
        }

        [TestMethod]
        public async Task DataManazinesController_GetFiles_NotNull()
        {
            var controller = new DataMagazinesController();

            var magazineInfos = controller.GetDataMagazinesInfo();
            var  files = new List<byte[]>();
            foreach (var info in magazineInfos)
            {
                var file = await controller.GetDataMagazineAsync(info.Path);
                files.Add(file);
            }

            Assert.IsTrue(files.Any());

            foreach (var file in files)
            {
                Assert.IsNotNull(file);
            }
        }
    }
}
