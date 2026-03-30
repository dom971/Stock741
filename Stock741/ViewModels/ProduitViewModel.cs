using System.Collections.ObjectModel;
using System.Windows.Input;
using Stock741.Models;
using Stock741.Repositories;
using Stock741.Commands;

namespace Stock741.ViewModels
{
    public class ProduitViewModel : BaseViewModel
    {
        private readonly ProduitRepository _repository;

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

        public ICommand AjouterProduitCommand { get; }
        public ICommand ModifierProduitCommand { get; }
        public ICommand SupprimerProduitCommand { get; }

        public ProduitViewModel(ProduitRepository repository)
        {
            _repository = repository;

            Produits = new ObservableCollection<Produit>(_repository.GetAll());

            AjouterProduitCommand = new RelayCommand(_ => AjouterProduit());
            ModifierProduitCommand = new RelayCommand(_ => ModifierProduit());
            SupprimerProduitCommand = new RelayCommand(_ => SupprimerProduit());
        }

        private void AjouterProduit()
        {
            if (string.IsNullOrWhiteSpace(Nom)) return;

            var produit = new Produit
            {
                Nom = Nom,
                Quantite = Quantite
            };

            _repository.Add(produit);
            Produits.Add(produit);

            Nom = "";
            Quantite = 0;
        }

        private void ModifierProduit()
        {
            if (ProduitSelectionne == null) return;

            ProduitSelectionne.Nom = Nom;
            ProduitSelectionne.Quantite = Quantite;

            _repository.Update(ProduitSelectionne);
        }

        private void SupprimerProduit()
        {
            if (ProduitSelectionne == null) return;

            _repository.Delete(ProduitSelectionne);
            Produits.Remove(ProduitSelectionne);
        }
    }
}