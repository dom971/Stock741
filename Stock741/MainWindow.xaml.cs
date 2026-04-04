using System.Windows;
using Stock741.ViewModels;

namespace Stock741.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService(typeof(MainViewModel)) as MainViewModel;
            if (!string.IsNullOrWhiteSpace(App.CheminLogo) &&
                System.IO.File.Exists(App.CheminLogo))
            {
                Logo.Source = new System.Windows.Media.Imaging.BitmapImage(
                    new System.Uri(App.CheminLogo));
            }
        }
    }
}