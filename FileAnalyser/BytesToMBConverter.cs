using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FileAnalyser
{
    public class BytesToMBConverter : IValueConverter
    {
        private static readonly string[] SizeSuffixes = { "Bytes", "KB", "MB", "GB", "TB" };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long bytes)
            {
                int order = 0;
                double tempBytes = bytes;

                while (tempBytes >= 1024 && order < SizeSuffixes.Length - 1)
                {
                    order++;
                    tempBytes /= 1024;
                }

                return $"{tempBytes:F2} {SizeSuffixes[order]}";
            }

            return "Ungültig";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
