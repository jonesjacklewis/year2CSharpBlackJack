using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace OOPA1.Helpers
{
    // Loosley based on https://wpf-tutorial.com/data-binding/value-conversion-with-ivalueconverter/
    internal class PencePoundConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int pence)
            {
                return pence / 100.0; // convertion rate to pounds
            }
            return value; // default return
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double pounds)
            {
                return (int)(pounds * 100); // convert to pence
            }
            return value; //  default result
        }
    }
}
