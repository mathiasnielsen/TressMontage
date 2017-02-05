using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TressMontage.Utitlies.Extensions
{
    public static class StringExtensions
    {
        public static string GetFileType(this string fileName)
        {
            return fileName.Split('.').Last();
        }
    }
}
