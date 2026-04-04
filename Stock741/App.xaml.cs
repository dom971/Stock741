using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock741.Data;
using Stock741.Repositories;
using Stock741.ViewModels;
using Stock741.Views;
using System;
using System.Windows;

namespace Stock741
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static string CheminBasePhotos { get; private set; }
        public static string CheminBaseFiches { get; private set; }

        public static string CheminLogo { get; private set; }

        // ⚠️ Cette méthode doit exister exactement comme ça

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            CheminBasePhotos = config["Photos:CheminBase"];
            CheminBaseFiches = config["Fiches:CheminBase"];
            CheminLogo = config["Logo:CheminLogo"];

            var services = new ServiceCollection();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("Default")));
           
            services.AddTransient<MarqueRepository>();            
            services.AddTransient<MarqueViewModel>();

            services.AddTransient<MaterielRepository>();
            services.AddTransient<MaterielViewModel>();

            services.AddTransient<ModeleRepository>();
            services.AddTransient<ModeleViewModel>();

            services.AddTransient<LieuRepository>();
            services.AddTransient<LieuViewModel>();

            services.AddTransient<FicheRepository>();
            services.AddTransient<FicheViewModel>();

            services.AddTransient<StatutRepository>();
            services.AddTransient<StatutViewModel>();

            services.AddTransient<FournisseurRepository>();
            services.AddTransient<FournisseurViewModel>();

            services.AddTransient<OperateurRepository>();
            services.AddTransient<OperateurViewModel>();

            services.AddTransient<ForfaitRepository>();
            services.AddTransient<ForfaitViewModel>();

            services.AddTransient<MainViewModel>();

            services.AddTransient<RequeteViewModel>();

            ServiceProvider = services.BuildServiceProvider();

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }


    }
}