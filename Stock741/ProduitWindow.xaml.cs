using System.Windows;
using Stock741.ViewModels;

namespace Stock741
{
    public partial class ProduitWindow : Window
    {
        public ProduitWindow()
        {
            InitializeComponent();
            DataContext = new ProduitViewModel();
        }
    }
}
