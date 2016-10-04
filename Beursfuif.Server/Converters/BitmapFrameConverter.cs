using Beursfuif.Server.DataAccess;
using System;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Beursfuif.Server.Converters
{
    public sealed class BitmapFrameConverter : IValueConverter
    {
        //doubles purely to facilitate easy data binding

        public double DecodePixelWidth { get; set; }

        public double DecodePixelHeight { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string path = value as string;

            if(!File.Exists(path))
            {
                path = BeursfuifPaths.AssetsPath + path;
            }

            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                //create new stream and create bitmap frame
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new FileStream(path, FileMode.Open, FileAccess.Read);
                bitmapImage.DecodePixelWidth = (int)DecodePixelWidth;
                bitmapImage.DecodePixelHeight = (int)DecodePixelHeight;
                //load the image now so we can immediately dispose of the stream
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                //clean up the stream to avoid file access exceptions when attempting to delete images
                bitmapImage.StreamSource.Dispose();

                return bitmapImage;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

}
