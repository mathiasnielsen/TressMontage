using System;
using System.Globalization;
using TressMontage.Utilities;
using Xamarin.Forms;

namespace TressMontage.Client.Converters
{
    public class FileTypeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DirectiveTypes)
            {
                var directiveType = (DirectiveTypes)value;

                switch (directiveType)
                { 
                    case DirectiveTypes.Folder:
                        return "icon_folder.png";

                    case DirectiveTypes.File:
                        return "icon_file.png";
                }

                return string.Empty;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
