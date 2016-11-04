﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<string>> GetFileNamesAsync()
        {
            var fileNames = await executor.Get<List<string>>(baseUrl + "datamagazines/fileNames");
            return fileNames;
        }

        public async Task<List<byte[]>> GetFilesAsync()
        {
            var fileNames = await GetFileNamesAsync();
            var files = new List<byte[]>();
            foreach (var fileName in fileNames)
            {
                var fileNameAsBytes = Encoding.UTF8.GetBytes(fileName);
                var encodedFileName = Convert.ToBase64String(fileNameAsBytes);
                var result = await executor.Get<byte[]>(baseUrl + $"datamagazines/{encodedFileName}");
                files.Add(result);
            }

            return files;
        }
    }
}
