using System;
using System.Globalization;
using Xamarin.Forms;

namespace TressMontage.Client
{
    public class ProgressToPercentageTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float)
            {
                var progress = (float)value;
                var progressInPercentage = progress * 100;

                return $"{(int)progressInPercentage} %";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
