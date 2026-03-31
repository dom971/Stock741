using System.Collections.ObjectModel;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class ProduitViewModel : BaseViewModel
    {
        private readonly ProduitRepository _repository;
        private readonly MarqueRepository _marqueRepository;

        public ObservableCollection<Produit> Produits { get; set; }
        public ObservableCollection<Marque> Marques { get; set; }

        private string _nomSelectionne;
        public string NomSelectionne
        {
            get => _nomSelectionne;
            set { _nomSelectionne = value; OnPropertyChanged(); }
        }

        private bool _actifSelectionne = true;
        public bool ActifSelectionne
        {
            get => _actifSelectionne;
            set { _actifSelectionne = value; OnPropertyChanged(); }
        }

        private Marque _marqueSelectionnee;
        public Marque MarqueSelectionnee
        {
            get => _marqueSelectionnee;
            set { _marqueSelectionnee = value; OnPropertyChanged(); }
        }

        private Produit _produitSelectionne;
        public Produit ProduitSelectionne
        {
            get => _produitSelectionne;
            set
            {
                _produitSelectionne = value;
                OnPropertyChanged();

                if (value != null)
                {
                    NomSelectionne = value.Nom;
                    ActifSelectionne = value.Actif;
                    MarqueSelectionnee = value.Marque;
                }
            }
        }

        public ICommand AjouterProduitCommand { get; }
        public ICommand ModifierProduitCommand { get; }
        public ICommand SupprimerProduitCommand { get; }

        public ProduitViewModel(ProduitRepository repository, MarqueRepository marqueRepository)
        {
            _repository = repository;
            _marqueRepository = marqueRepository;

            Produits = new ObservableCollection<Produit>(_repository.GetAll());
            Marques = new ObservableCollection<Marque>(_marqueRepository.GetAll());

            AjouterProduitCommand = new RelayCommand(AjouterProduit, CanAjouterProduit);
            ModifierProduitCommand = new RelayCommand(ModifierProduit, CanModifierProduit);
            SupprimerProduitCommand = new RelayCommand(SupprimerProduit, CanSupprimerProduit);
        }

        private bool CanAjouterProduit(object obj) => !string.IsNullOrWhiteSpace(NomSelectionne) && MarqueSelectionnee != null;
        private bool CanModifierProduit(object obj) => ProduitSelectionne != null && MarqueSelectionnee != null;
        private bool CanSupprimerProduit(object obj) => ProduitSelectionne != null;

        private void AjouterProduit(object obj)
        {
            var produit = new Produit
            {
                Nom = NomSelectionne,
                Actif = ActifSelectionne,
                Marque = MarqueSelectionnee,
                MarqueId = MarqueSelectionnee.Id
            };

            _repository.Add(produit);
            Produits.Add(produit);

            NomSelectionne = string.Empty;
            ActifSelectionne = true;
            MarqueSelectionnee = null;
        }

        private void ModifierProduit(object obj)
        {
            if (ProduitSelectionne == null || MarqueSelectionnee == null) return;

            ProduitSelectionne.Nom = NomSelectionne;
            ProduitSelectionne.Actif = ActifSelectionne;
            ProduitSelectionne.Marque = MarqueSelectionnee;
            ProduitSelectionne.MarqueId = MarqueSelectionnee.Id;

            _repository.Update(ProduitSelectionne);

            int index = Produits.IndexOf(ProduitSelectionne);
            if (index >= 0)
            {
                Produits[index] = ProduitSelectionne;
            }
        }

        private void SupprimerProduit(object obj)
        {
            if (ProduitSelectionne == null) return;

            _repository.Delete(ProduitSelectionne);
            Produits.Remove(ProduitSelectionne);

            ProduitSelectionne = null;
            NomSelectionne = string.Empty;
            ActifSelectionne = true;
            MarqueSelectionnee = null;
        }
    }
}

//using System.Collections.ObjectModel;
//using System.Windows.Input;
//using Stock741.Commands;
//using Stock741.Models;
//using Stock741.Repositories;

//namespace Stock741.ViewModels
//{
//    public class ProduitViewModel : BaseViewModel
//    {
//        private readonly ProduitRepository _repository;
//        private readonly MarqueRepository _marqueRepository;

//        public ObservableCollection<Produit> Produits { get; set; }
//        public ObservableCollection<Marque> Marques { get; set; }

//        // Champs pour l'ajout
//        private string _nouveauNom;
//        public string NouveauNom
//        {
//            get => _nouveauNom;
//            set { _nouveauNom = value; OnPropertyChanged(); }
//        }

//        private Marque _nouvelleMarque;
//        public Marque NouvelleMarque
//        {
//            get => _nouvelleMarque;
//            set { _nouvelleMarque = value; OnPropertyChanged(); }
//        }

//        private bool _nouveauActif = true;
//        public bool NouveauActif
//        {
//            get => _nouveauActif;
//            set { _nouveauActif = value; OnPropertyChanged(); }
//        }

//        public ICommand AjouterProduitCommand { get; }

//        public ProduitViewModel(ProduitRepository repository, MarqueRepository marqueRepo)
//        {
//            _repository = repository;
//            _marqueRepository = marqueRepo;

//            Produits = new ObservableCollection<Produit>(_repository.GetAll());
//            Marques = new ObservableCollection<Marque>(_marqueRepository.GetAll());

//            AjouterProduitCommand = new RelayCommand(AjouterProduit, CanAjouterProduit);
//        }

//        private bool CanAjouterProduit(object obj)
//        {
//            return !string.IsNullOrWhiteSpace(NouveauNom) && NouvelleMarque != null;
//        }

//        private void AjouterProduit(object obj)
//        {
//            var produit = new Produit
//            {
//                Nom = NouveauNom,
//                MarqueId = NouvelleMarque.Id,
//                Actif = NouveauActif
//            };

//            _repository.Add(produit);
//            Produits.Add(produit);

//            // Reset champs
//            NouveauNom = string.Empty;
//            NouvelleMarque = null;
//            NouveauActif = true;
//        }
//    }
//}



//using System.Collections.ObjectModel;
//using System.Windows.Input;
//using Stock741.Models;
//using Stock741.Repositories;
//using Stock741.Commands;

//namespace Stock741.ViewModels
//{
//    public class ProduitViewModel : BaseViewModel
//    {
//        private readonly ProduitRepository _repository;

//        public ObservableCollection<Produit> Produits { get; set; }

//        private string _nom;
//        public string Nom
//        {
//            get => _nom;
//            set { _nom = value; OnPropertyChanged(); }
//        }

//        private int _quantite;
//        public int Quantite
//        {
//            get => _quantite;
//            set { _quantite = value; OnPropertyChanged(); }
//        }

//        private Produit _produitSelectionne;
//        public Produit ProduitSelectionne
//        {
//            get => _produitSelectionne;
//            set
//            {
//                _produitSelectionne = value;
//                OnPropertyChanged();

//                if (_produitSelectionne != null)
//                {
//                    Nom = _produitSelectionne.Nom;
//                    //Quantite = _produitSelectionne.Quantite;
//                }
//            }
//        }

//        public ICommand AjouterProduitCommand { get; }
//        public ICommand ModifierProduitCommand { get; }
//        public ICommand SupprimerProduitCommand { get; }

//        public ProduitViewModel(ProduitRepository repository)
//        {
//            _repository = repository;

//            Produits = new ObservableCollection<Produit>(_repository.GetAll());

//            AjouterProduitCommand = new RelayCommand(_ => AjouterProduit());
//            ModifierProduitCommand = new RelayCommand(_ => ModifierProduit());
//            SupprimerProduitCommand = new RelayCommand(_ => SupprimerProduit());
//        }

//        private void AjouterProduit()
//        {
//            if (string.IsNullOrWhiteSpace(Nom)) return;

//            var produit = new Produit
//            {
//                Nom = Nom,
//                //Quantite = Quantite
//            };

//            _repository.Add(produit);
//            Produits.Add(produit);

//            Nom = "";
//            Quantite = 0;
//        }

//        private void ModifierProduit()
//        {
//            if (ProduitSelectionne == null) return;

//            ProduitSelectionne.Nom = Nom;
//            //ProduitSelectionne.Quantite = Quantite;

//            _repository.Update(ProduitSelectionne);
//        }

//        private void SupprimerProduit()
//        {
//            if (ProduitSelectionne == null) return;

//            _repository.Delete(ProduitSelectionne);
//            Produits.Remove(ProduitSelectionne);
//        }
//    }
//}