using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Client.Core.Http.Mappers;
using TressMontage.Entities;

namespace TressMontage.Client.Core.Http.Clients
{
    public class TressMontageApi : ITressMontageApi
    {
        private const string baseUrl = "http://tressmontageapp.azurewebsites.net/api/";

        private readonly IHttpRequestExecutor executor;
        private readonly FileDirectiveMapper fileDirectiveMapper;

        public TressMontageApi(IHttpClientFactory httpClientFactory)
        {
            executor = new HttpRequestExecutor(httpClientFactory);

            fileDirectiveMapper = new FileDirectiveMapper();
        }

        public async Task<List<FileDirective>> GetFileNamesAsync()
        {
            try
            {
                var fileDirectives = await executor.Get<List<FileDirective>>(baseUrl + "datamagazines/fileNames");

                var mappedFileDirectives = fileDirectiveMapper.MapFileDirectives(fileDirectives);

                return mappedFileDirectives;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed getting filedirectives");
            }
        }

        public async Task<List<byte[]>> GetFilesAsync()
        {
            var fileNames = await GetFileNamesAsync();
            var files = new List<byte[]>();
            foreach (var fileName in fileNames)
            {
                var fileAsByteArray = await GetFileAsync(fileName.DirectoryWithName, fileName.ExtensionNoDot);
                files.Add(fileAsByteArray);
            }

            return files;
        }

        public async Task<byte[]> GetFileAsync(string fileName, string extension)
        {
            var result = await executor.Get<byte[]>(baseUrl + $"datamagazines/{fileName}/{extension}");
            return result;
        }
    }
}
