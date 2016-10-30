using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Entities;

namespace TressMontage.Client.Core.Services
{
    public interface IFileInfoManager
    {
        Task<List<Folder>> GetCurrentFolderPathsAsync(string currentFolderPath = "");
    }
}
