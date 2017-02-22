using System.IO;

namespace TressMontage.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static string GetPlatformSpecificPath(this string path)
        {
            var splittedPath = path.Split('/');

            var combinedPath = Path.Combine(splittedPath);

            return combinedPath;
        }
    }
}
