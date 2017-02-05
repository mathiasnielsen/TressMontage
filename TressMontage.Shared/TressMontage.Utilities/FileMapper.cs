using TressMontage.Utitlies.Extensions;

namespace TressMontage.Utilities
{
    public class FileMapper
    {
        public FileTypes GetFileType(string fileName)
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
