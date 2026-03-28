using System.Windows;
using Stock741.Model;

namespace Stock741
{
    public partial class MarqueWindow : Window
    {
        private readonly MarqueViewModel _vm = new MarqueViewModel();

        public MarqueWindow()
        {
            InitializeComponent();
            DataContext = _vm;
            _vm.LoadMarques();
        }

        private void BtnNouveau_Click(object sender, RoutedEventArgs e)
        {
            _vm.SelectedMarque = new Marque { Actif = true };
        }

        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
        {
            _vm.AddMarque(_vm.SelectedMarque);
        }

        private void BtnModifier_Click(object sender, RoutedEventArgs e)
        {
            _vm.UpdateMarque(_vm.SelectedMarque);
        }

        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            _vm.DeleteMarque(_vm.SelectedMarque);
        }
    }
}




//using System.Windows;
//using Stock741.Model;

//namespace Stock741
//{
//    public partial class MarqueWindow : Window
//    {
//        private readonly MarqueViewModel _vm = new MarqueViewModel();

//        public MarqueWindow()
//        {
//            InitializeComponent();
//            DataContext = _vm;
//            _vm.LoadMarques();
//        }

//        private void BtnNouveau_Click(object sender, RoutedEventArgs e)
//        {
//            // INotifyPropertyChanged suffit, plus besoin du hack DataContext = null
//            _vm.SelectedMarque = new Marque { Actif = true };

//        }

//        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
//        {
//            _vm.AddMarque(_vm.SelectedMarque);
//        }

//        private void BtnModifier_Click(object sender, RoutedEventArgs e)
//        {
//            _vm.UpdateMarque(_vm.SelectedMarque);
//        }

//        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
//        {
//            _vm.DeleteMarque(_vm.SelectedMarque);
//        }
//    }
//}







//using System;
//using System.Collections.ObjectModel;
//using System.Windows;
//using Microsoft.Data.SqlClient; // Pour .NET 10
//using Stock741.Model;

//namespace Stock741
//{
//    public partial class MarqueWindow : Window
//    {
//        private MarqueViewModel vm = new MarqueViewModel();



//        public MarqueWindow()
//        {
//            InitializeComponent(); // Obligatoire pour lier le XAML
//            DataContext = vm;
//            vm.LoadMarques();
//        }

//        private void BtnNouveau_Click(object sender, RoutedEventArgs e)
//        {
//            vm.SelectedMarque = new Marque();
//            DataContext = null;
//            DataContext = vm; // refresh binding
//        }

//        private void BtnAjouter_Click(object sender, RoutedEventArgs e)
//        {
//            vm.AddMarque(vm.SelectedMarque);
//        }

//        private void BtnModifier_Click(object sender, RoutedEventArgs e)
//        {
//            vm.UpdateMarque(vm.SelectedMarque);
//        }

//        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
//        {
//            vm.DeleteMarque(vm.SelectedMarque);
//        }


//        //private void BtnAjouter_Click(object sender, RoutedEventArgs e)
//        //{
//        //    Marque m = new Marque { Nom = "Nouvelle Marque", Description = "" };
//        //    vm.AddMarque(m);
//        //}

//        //private void BtnModifier_Click(object sender, RoutedEventArgs e)
//        //{
//        //    if (DataGridMarques.SelectedItem is Marque m)
//        //    {
//        //        vm.UpdateMarque(m);
//        //    }
//        //}

//        //private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
//        //{
//        //    if (DataGridMarques.SelectedItem is Marque m)
//        //    {
//        //        vm.DeleteMarque(m);
//        //    }
//        //}
//    }
//}
