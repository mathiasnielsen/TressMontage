using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TressMontage.Client.UWP.Helpers
{
    public class FileAccessHelper
    {
        public static string GetLoalDirectory()
        {
            return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        }

        public static string GetLocalFilePath(string filename)
        {
            string path = GetLoalDirectory();

            return System.IO.Path.Combine(path, filename);
        }
    }
}
