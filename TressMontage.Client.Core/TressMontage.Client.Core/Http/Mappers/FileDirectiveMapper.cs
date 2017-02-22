using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Entities;
using TressMontage.Utilities.Extensions;

namespace TressMontage.Client.Core.Http.Mappers
{
    public class FileDirectiveMapper
    {
        public List<FileDirective> MapFileDirectives(List<FileDirective> fileDirectives)
        {
            var mappedFileDirectives = fileDirectives.Select(x => MapFileDirective(x));

            return mappedFileDirectives.ToList();
        }

        public FileDirective MapFileDirective(FileDirective fileDirective)
        {
            fileDirective.BlobPath = fileDirective.BlobPath.GetPlatformSpecificPath();
            return fileDirective;
        }
    }
}
