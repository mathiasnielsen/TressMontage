using System.Collections.Generic;
using System.Threading.Tasks;
using TressMontage.Entities;

namespace TressMontage.Client.Core.Http.Clients
{
    public interface ITressMontageApi
    {
        Task<List<FileDirective>> GetFileNamesAsync();

        Task<byte[]> GetFileAsync(string fileName, string extension);

        Task<List<byte[]>> GetFilesAsync();
    }
}