using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using TressMontage.Entities;
using TressMontage.Utilities;

namespace TressMontage.Client.Core.Utilitites
{
    public class FileInfoMapper
    {
        public List<FileDirective> MapFolderToFileInfo(IList<IFolder> folders)
        {
            var fileInfos = folders.Select(x => new FileDirective() { BlobPath = x.Path, Type = DirectiveTypes.Folder });
            return fileInfos?.ToList() ?? new List<FileDirective>();
        }

        public List<FileDirective> MapFilesToFileInfo(IList<IFile> files)
        {
            var fileInfos = files.Select(x => new FileDirective() { BlobPath = x.Path, Type = DirectiveTypes.File });
            return fileInfos?.ToList() ?? new List<FileDirective>();
        }
    }
}
