using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Stock741.Data;
using Stock741.Repositories;
using Stock741.ViewModels;
using Stock741.Views;

namespace Stock741
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        // ⚠️ Cette méthode doit exister exactement comme ça
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 1️⃣ Configurer les services
            var services = new ServiceCollection();

            // DbContext (connection string dans AppDbContext)
            services.AddDbContext<AppDbContext>();

            // Repositories
            services.AddSingleton<ProduitRepository>();
            services.AddSingleton<MarqueRepository>();

            // ViewModels
            services.AddTransient<ProduitViewModel>();
            services.AddTransient<MarqueViewModel>();

            // Construire le fournisseur de services
            ServiceProvider = services.BuildServiceProvider();

            // 2️⃣ Créer et afficher la fenêtre principale
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}

//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Windows;
//using Stock741.Repositories;
//using Stock741.ViewModels;
//using Stock741.Views;

//namespace Stock741
//{
//    public partial class App : Application
//    {
//        public static IServiceProvider ServiceProvider { get; private set; }

//        protected override void OnStartup(StartupEventArgs e)
//        {
//            base.OnStartup(e); // ⚠️ Toujours appeler base.OnStartup

//            var services = new ServiceCollection();

//            services.AddDbContext<Data.AppDbContext>();
//            services.AddScoped<ProduitRepository>();
//            services.AddScoped<ProduitViewModel>();

//            ServiceProvider = services.BuildServiceProvider();

//            // Créer et afficher la fenêtre
//            var window = new ProduitWindow
//            {
//                DataContext = ServiceProvider.GetService<ProduitViewModel>()
//            };

//            window.Show(); // ⚠️ C’est ça qui affiche la fenêtre
//        }
//    }
//}