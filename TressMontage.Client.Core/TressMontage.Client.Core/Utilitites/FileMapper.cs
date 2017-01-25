using System.Linq;
using System.Collections.Generic;
using PCLStorage;
using TressMontage.Entities;
using TressMontage.Client.Core.Extensions;
using TressMontage.Enitites;

namespace TressMontage.Client.Utilities
{
    public class FileMapper
    {
        public List<FileInfo> MapFolderToFileInfo(IList<IFolder> folders)
        {
            var fileInfos = folders.Select(x => new FileInfo() { Name = x.Name, Path = x.Path, Type = FileTypes.FOLDER });
            return fileInfos?.ToList();
        }

        public List<FileInfo> MapFilesToFileInfo(IList<IFile> files)
        {
            var fileInfos = files.Select(x => new FileInfo() { Name = x.Name, Path = x.Path, Type = GetFileType(x.Name) });
            return fileInfos?.ToList();
        }

        private FileTypes GetFileType(string fileName)
        {
            var extension = fileName.GetFileType();
            switch (extension)
            {
                case "txt":
                    return FileTypes.TXT;

                case "pdf":
                    return FileTypes.PDF;

                default:
                    return FileTypes.UNKNOWN;
            }
        }
    }
}
