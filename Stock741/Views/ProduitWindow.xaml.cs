using Microsoft.Extensions.DependencyInjection;
using Stock741.ViewModels;
using System.Windows;

namespace Stock741.Views
{
    public partial class ProduitWindow : Window
    {
        public ProduitWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService<ProduitViewModel>();
        }
    }
}
