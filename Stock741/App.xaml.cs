using System.Windows;

namespace Stock741
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Ouvre MarqueWindow au démarrage
            //MainWindow window = new MainWindow();
            //MarqueWindow window = new MarqueWindow();
            ProduitWindow window = new ProduitWindow();
            window.Show();
        }
    }
}


//using System.Configuration;
//using System.Data;
//using System.Windows;

//namespace Stock741
//{
//    /// <summary>
//    /// Interaction logic for App.xaml
//    /// </summary>
//    public partial class App : Application
//    {
//    }

//}
