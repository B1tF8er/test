using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace App.Wpf.Helper
{
    public class ByteToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapSource bitmapSource = new BitmapImage();

            if (value != null && (value as byte[]).Length > 0)
            {
                ImageSourceConverter converter = new ImageSourceConverter();
                bitmapSource = (BitmapSource)converter.ConvertFrom(value);
            }
            return bitmapSource;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] byteArray = new byte[0];

            if (value != null)
            {
                ImageSourceConverter converter = new ImageSourceConverter();
                byteArray = (byte[])converter.ConvertFrom(value);
            }

            return byteArray;
        }
    }
}
