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
        }
    }
}