using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TressMontage.Utilities
{
    public static class FileInfoCombiner
    {
        public static string CombineFileName(Entities.FileInfoDTO fileInfoDTO)
        {
            return CombineFileInfo(fileInfoDTO.Path, fileInfoDTO.Name);
        }

        public static string CombineFileName(Entities.FileInfo fileInfo)
        {
            return CombineFileInfo(fileInfo.Path, fileInfo.Name);
        }

        private static string CombineFileInfo(string path, string name)
        {
            return Path.Combine(path, name);
        }
    }
}
