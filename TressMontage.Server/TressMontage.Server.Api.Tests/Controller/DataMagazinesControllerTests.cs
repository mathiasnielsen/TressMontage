using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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

            var fileInfo = new Entities.FileInfo()
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

            var path = FileInfoCombiner.CombineFileName(fileInfo);

            var response = await controller.GetDataMagazineAsync(path, fileInfo.Type.ToString());
            var data = await GetDataFromResponseAsync<byte[]>(response);

            var postedMagazineAsText = Convert.ToBase64String(data, 0, data.Length);

            Assert.IsTrue(dataMagazineAsText == postedMagazineAsText);
        }

        [TestMethod]
        public async Task DataManazinesController_GetBlobUrls_NotNull()
        {
            var controller = new DataMagazinesController();

            var result = controller.GetDataMagazinesInfo();
            var content = await result.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<FileInfoDTO>>(content);

            Assert.IsNotNull(result);
            Assert.IsTrue(data.Any());
        }

        ////[TestMethod]
        ////public async Task DataManazinesController_GetFiles_NotNull()
        ////{
        ////    var controller = new DataMagazinesController();

        ////    var response = controller.GetDataMagazinesInfo();
        ////    var data = await GetDataFromResponseAsync<List<FileInfoDTO>>(response);

        ////    var files = new List<byte[]>();
        ////    foreach (var info in data)
        ////    {
        ////        var fileInfoDTO = new FileInfoDTO() { Name = info.Name, Path = info.Path, Type = info.Type };
        ////        var magazineResponse = await controller.GetDataMagazineAsync(fileInfoDTO);
        ////        var magazineData = await GetDataFromResponseAsync<byte[]>(magazineResponse);

        ////        files.Add(magazineData);
        ////    }

        ////    Assert.IsTrue(files.Any());

        ////    foreach (var file in files)
        ////    {
        ////        Assert.IsNotNull(file);
        ////    }
        ////}

        private async Task<TDataType> GetDataFromResponseAsync<TDataType>(HttpResponseMessage response)
        {
            var contentAsString = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<TDataType>(contentAsString);

            return data;
        }
    }
}
