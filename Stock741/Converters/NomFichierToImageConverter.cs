using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Stock741.Converters
{
    public class NomFichierToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string nomFichier || string.IsNullOrWhiteSpace(nomFichier))
                return null;

            var cheminComplet = Path.Combine(App.CheminBasePhotos, nomFichier);

            if (!File.Exists(cheminComplet))
                return null;

            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(cheminComplet);
                image.EndInit();
                return image;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
