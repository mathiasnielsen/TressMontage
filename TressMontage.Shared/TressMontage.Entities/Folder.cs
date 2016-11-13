using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TressMontage.Entities
{
    public class Folder : FileInfo
    {
        public int SubFoldersCount { get; set; }
    }
}
