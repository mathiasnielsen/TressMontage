using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Entities;

namespace TressMontage.Client.Core.Http.Clients
{
    public class TressMontageApi : ITressMontageApi
    {
        private const string baseUrl = "http://tressmontageapp.azurewebsites.net/api/";
        private readonly IHttpRequestExecutor executor;

        public TressMontageApi(IHttpClientFactory httpClientFactory)
        {
            executor = new HttpRequestExecutor(httpClientFactory);
        }

        public async Task<List<FileInfoDTO>> GetFileNamesAsync()
        {
            try
            {
                var fileNames = await executor.Get<List<FileInfoDTO>>(baseUrl + "datamagazines/fileNames");
                return fileNames;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not get filenames");
            }
        }

        public async Task<List<byte[]>> GetFilesAsync()
        {
            var fileNames = await GetFileNamesAsync();
            var files = new List<byte[]>();
            foreach (var fileName in fileNames)
            {
                var fileAsByteArray = await GetFileAsync(fileName.Path);
                files.Add(fileAsByteArray);
            }

            return files;
        }

        public async Task<byte[]> GetFileAsync(string fileName)
        {
            var result = await executor.Get<byte[]>(baseUrl + $"datamagazines/{fileName}");
            return result;
        }
    }
}
