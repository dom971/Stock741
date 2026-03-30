using Microsoft.Extensions.DependencyInjection;
using Stock741.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Stock741.Views
{
    /// <summary>
    /// Logique d'interaction pour MarqueWindow.xaml
    /// </summary>
    public partial class MarqueWindow : Window
    {
        public MarqueWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetService<MarqueViewModel>();
        }
    }
}
