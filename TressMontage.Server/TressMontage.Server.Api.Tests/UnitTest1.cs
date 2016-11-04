﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TressMontage.Server.Api.Controllers;

namespace TressMontage.Server.Api.Tests
{
    [TestClass]
    public class UnitTest1
    {
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
                var file = await controller.GetDataMagazineAsync(blobName);
                files.Add(file);
            }

            foreach (var file in files)
            {
                Assert.IsNotNull(file);
            }
        }
    }
}
