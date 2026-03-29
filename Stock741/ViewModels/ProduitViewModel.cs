using Stock741.Commands;
using Stock741.Models; // Assure-toi que Produit.cs est dans Models
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Stock741.ViewModels
{
    public class ProduitViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Produit> Produits { get; set; }

        private string _nom;
        public string Nom
        {
            get => _nom;
            set { _nom = value; OnPropertyChanged(); }
        }

        private int _quantite;
        public int Quantite
        {
            get => _quantite;
            set { _quantite = value; OnPropertyChanged(); }
        }

        //private Produit _produitSelectionne;
        //public Produit ProduitSelectionne
        //{
        //    get => _produitSelectionne;
        //    set { _produitSelectionne = value; OnPropertyChanged(); }
        //}

        private Produit _produitSelectionne;
        public Produit ProduitSelectionne
        {
            get => _produitSelectionne;
            set
            {
                _produitSelectionne = value;
                OnPropertyChanged();

                if (_produitSelectionne != null)
                {
                    Nom = _produitSelectionne.Nom;
                    Quantite = _produitSelectionne.Quantite;
                }
            }
        }


        // Commandes
        public ICommand AjouterProduitCommand { get; }
        public ICommand SupprimerProduitCommand { get; }

        public ICommand ModifierProduitCommand { get; }

        public int NombreProduits => Produits.Count;





        public ProduitViewModel()
        {
            Produits = new ObservableCollection<Produit>();

            AjouterProduitCommand = new RelayCommand(_ => AjouterProduit());
            SupprimerProduitCommand = new RelayCommand(_ => SupprimerProduit());
            ModifierProduitCommand = new RelayCommand(_ => ModifierProduit());
        }

        private void AjouterProduit()
        {
            if (string.IsNullOrWhiteSpace(Nom)) return;

            Produits.Add(new Produit { Nom = Nom, Quantite = Quantite });
            OnPropertyChanged(nameof(NombreProduits));

            // Réinitialiser les champs
            Nom = "";
            Quantite = 0;
        }

        private void SupprimerProduit()
        {
            if (ProduitSelectionne == null) return;

            Produits.Remove(ProduitSelectionne);
            OnPropertyChanged(nameof(NombreProduits));
        }

        private void ModifierProduit()
        {
            if (ProduitSelectionne == null) return;

            ProduitSelectionne.Nom = Nom;
            ProduitSelectionne.Quantite = Quantite;

            OnPropertyChanged(nameof(Produits));
        }


        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}