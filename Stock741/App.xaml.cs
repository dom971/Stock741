using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using Stock741.Repositories;
using Stock741.ViewModels;

namespace Stock741
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e); // ⚠️ Toujours appeler base.OnStartup

            var services = new ServiceCollection();

            services.AddDbContext<Data.AppDbContext>();
            services.AddScoped<ProduitRepository>();
            services.AddScoped<ProduitViewModel>();

            ServiceProvider = services.BuildServiceProvider();

            // Créer et afficher la fenêtre
            var window = new ProduitWindow
            {
                DataContext = ServiceProvider.GetService<ProduitViewModel>()
            };

            window.Show(); // ⚠️ C’est ça qui affiche la fenêtre
        }
    }
}