﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace TressMontage.Client.Core.Http.Clients
{
    public interface ITressMontageApi
    {
        Task<List<string>> GetFileNamesAsync();

        Task<byte[]> GetFileAsync(string fileName);

        Task<List<byte[]>> GetFilesAsync();
    }
}