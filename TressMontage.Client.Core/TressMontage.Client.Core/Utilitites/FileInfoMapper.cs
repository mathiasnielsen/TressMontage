using System.Collections.Generic;
using System.Linq;
using PCLStorage;
using TressMontage.Entities;
using TressMontage.Utilities;

namespace TressMontage.Client.Utilities
{
    public class FileInfoMapper
    {
        public List<FileInfo> MapFolderToFileInfo(IList<IFolder> folders)
        {
            var fileInfos = folders.Select(x => new FileInfo() { Name = x.Name, Path = x.Path, Type = FileTypes.FOLDER });
            return fileInfos?.ToList();
        }

        public List<FileInfo> MapFilesToFileInfo(IList<IFile> files)
        {
            var fileMapper = new TressMontage.Utilities.FileMapper();
            var fileInfos = files.Select(x => new FileInfo() { Name = x.Name, Path = x.Path, Type = fileMapper.GetFileType(x.Name) });
            return fileInfos?.ToList();
        }
    }
}
