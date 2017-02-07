using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using TressMontage.Entities;
using TressMontage.Server.Api.Blobs;

namespace TressMontage.Server.Api.Controllers
{
    [RoutePrefix("api")]
    public class DataMagazinesController : ApiController
    {
        [Route("datamagazines/fileNames")]
        [HttpGet]
        public HttpResponseMessage GetDataMagazinesInfo()
        {
            var blobManager = new BlobManager();

            var blobs = blobManager.GetBlobsInContainer();

            var blobsAsFileInfoDTO = new List<FileDirective>();
            foreach (var blob in blobs)
            {
                var fileInfoDto = blobManager.GetFileInfoFromBlob(blob);
                blobsAsFileInfoDTO.Add(fileInfoDto);
            }

            var response = CreateResponse(blobsAsFileInfoDTO);

            return response;
        }

        [Route("datamagazines/{blobPath}/{extension}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetDataMagazineAsync(string blobPath, string extension)
        {
            var blobManager = new BlobManager();

            var result = await blobManager.GetBlobAsync(blobPath + "." + extension);
            var response = CreateResponse(result);

            return response;
        }

        [Route("datamagazines")]
        [HttpPost]
        public async Task<HttpResponseMessage> PostDataMagazineAsync(FileDTO fileDto)
        {
            var blobManager = new BlobManager();

            var result = await blobManager.UploadBlobIntoContainer(fileDto);
            var response = CreateResponse(result);

            return response;
        }

        private HttpResponseMessage CreateResponse<TData>(TData data)
        {
            var response = new HttpResponseMessage();

            var serializedData = JsonConvert.SerializeObject(data);
            response.Content = new StringContent(serializedData);

            return response;
        }
    }
}