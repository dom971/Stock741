using Stock741.Views;
using System.Windows;

namespace Stock741
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnProduits_Click(object sender, RoutedEventArgs e)
        {
            var fenetreProduit = new ProduitWindow();
            fenetreProduit.Show();
        }

        private void BtnMarques_Click(object sender, RoutedEventArgs e)
        {
            var fenetreMarque = new MarqueWindow();
            fenetreMarque.Show();
        }

        private void BtnQuitter_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}