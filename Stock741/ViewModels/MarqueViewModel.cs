using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class MarqueViewModel : BaseViewModel, IDataErrorInfo
    {
        private readonly MarqueRepository _repository;

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
            set
            {
                _marqueSelectionnee = value;
                OnPropertyChanged();
                if (value != null)
                {
                    NomSelectionne = value.Nom;
                    ActifSelectionne = value.Actif;
                }
            }
        }

        public ICommand AjouterMarqueCommand { get; }
        public ICommand ModifierMarqueCommand { get; }
        public ICommand SupprimerMarqueCommand { get; }

        public MarqueViewModel(MarqueRepository repository)
        {
            _repository = repository;
            Marques = new ObservableCollection<Marque>(_repository.GetAll());

            AjouterMarqueCommand = new RelayCommand(AjouterMarque, CanAjouterMarque);
            ModifierMarqueCommand = new RelayCommand(ModifierMarque, CanModifierMarque);
            SupprimerMarqueCommand = new RelayCommand(SupprimerMarque, CanSupprimerMarque);
        }

        // ----------------------
        // Validation nom unique
        // ----------------------
        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(NomSelectionne))
                {
                    if (string.IsNullOrWhiteSpace(NomSelectionne))
                        return "Nom obligatoire";

                    bool exists = Marques.Any(m =>
                        m.Nom.ToLower() == NomSelectionne.ToLower() &&
                        (MarqueSelectionnee == null || m.Id != MarqueSelectionnee.Id));

                    if (exists)
                        return "Nom déjà utilisé";
                }

                return null;
            }
        }

        public string Error => null;

        // ----------------------
        // Commandes
        // ----------------------
        private bool CanAjouterMarque(object obj) => string.IsNullOrWhiteSpace(this[nameof(NomSelectionne)]);
        private bool CanModifierMarque(object obj) => MarqueSelectionnee != null && string.IsNullOrWhiteSpace(this[nameof(NomSelectionne)]);
        private bool CanSupprimerMarque(object obj) => MarqueSelectionnee != null;

        private void AjouterMarque(object obj)
        {
            // Vérification avant ajout
            if (string.IsNullOrWhiteSpace(NomSelectionne)) return;

            bool exists = Marques.Any(m => m.Nom.ToLower() == NomSelectionne.ToLower());
            if (exists) return;

            var marque = new Marque
            {
                Nom = NomSelectionne,
                Actif = ActifSelectionne
            };

            _repository.Add(marque);
            Marques.Add(marque);

            // Réinitialiser les champs
            NomSelectionne = string.Empty;
            ActifSelectionne = true;

            // Rafraîchir DataGrid
            CollectionViewSource.GetDefaultView(Marques).Refresh();
        }

        private void ModifierMarque(object obj)
        {
            if (MarqueSelectionnee == null) return;

            // Vérification nom unique
            bool exists = Marques.Any(m =>
                m.Nom.ToLower() == NomSelectionne.ToLower() &&
                m.Id != MarqueSelectionnee.Id);
            if (exists) return;

            MarqueSelectionnee.Nom = NomSelectionne;
            MarqueSelectionnee.Actif = ActifSelectionne;

            _repository.Update(MarqueSelectionnee);

            // Rafraîchir DataGrid
            CollectionViewSource.GetDefaultView(Marques).Refresh();
        }

        private void SupprimerMarque(object obj)
        {
            if (MarqueSelectionnee == null) return;

            _repository.Delete(MarqueSelectionnee);
            Marques.Remove(MarqueSelectionnee);

            // Rafraîchir DataGrid
            CollectionViewSource.GetDefaultView(Marques).Refresh();

            // Réinitialiser champs
            MarqueSelectionnee = null;
            NomSelectionne = string.Empty;
            ActifSelectionne = true;
        }
    }
}


//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Windows.Data;
//using System.Windows.Input;
//using Stock741.Commands;
//using Stock741.Models;
//using Stock741.Repositories;

//namespace Stock741.ViewModels
//{
//    public class MarqueViewModel : BaseViewModel, IDataErrorInfo
//    {
//        private readonly MarqueRepository _repository;

//        public ObservableCollection<Marque> Marques { get; set; }

//        private string _nomSelectionne;
//        public string NomSelectionne
//        {
//            get => _nomSelectionne;
//            set { _nomSelectionne = value; OnPropertyChanged(); }
//        }

//        private bool _actifSelectionne = true;
//        public bool ActifSelectionne
//        {
//            get => _actifSelectionne;
//            set { _actifSelectionne = value; OnPropertyChanged(); }
//        }

//        private Marque _marqueSelectionnee;
//        public Marque MarqueSelectionnee
//        {
//            get => _marqueSelectionnee;
//            set
//            {
//                _marqueSelectionnee = value;
//                OnPropertyChanged();
//                if (value != null)
//                {
//                    NomSelectionne = value.Nom;
//                    ActifSelectionne = value.Actif;
//                }
//            }
//        }

//        public ICommand AjouterMarqueCommand { get; }
//        public ICommand ModifierMarqueCommand { get; }
//        public ICommand SupprimerMarqueCommand { get; }

//        public MarqueViewModel(MarqueRepository repository)
//        {
//            _repository = repository;
//            Marques = new ObservableCollection<Marque>(_repository.GetAll());

//            AjouterMarqueCommand = new RelayCommand(AjouterMarque, CanAjouterMarque);
//            ModifierMarqueCommand = new RelayCommand(ModifierMarque, CanModifierMarque);
//            SupprimerMarqueCommand = new RelayCommand(SupprimerMarque, CanSupprimerMarque);
//        }

//        // ----------------------
//        // Validation nom unique
//        // ----------------------
//        public string this[string columnName]
//        {
//            get
//            {
//                if (columnName == nameof(NomSelectionne))
//                {
//                    if (string.IsNullOrWhiteSpace(NomSelectionne))
//                        return "Nom obligatoire";

//                    bool exists = Marques.Any(m =>
//                        m.Nom.ToLower() == NomSelectionne.ToLower() &&
//                        (MarqueSelectionnee == null || m.Id != MarqueSelectionnee.Id));

//                    if (exists)
//                        return "Nom déjà utilisé";
//                }

//                return null;
//            }
//        }

//        public string Error => null;

//        // ----------------------
//        // Commandes
//        // ----------------------
//        private bool CanAjouterMarque(object obj) => string.IsNullOrWhiteSpace(this[nameof(NomSelectionne)]);
//        private bool CanModifierMarque(object obj) => MarqueSelectionnee != null && string.IsNullOrWhiteSpace(this[nameof(NomSelectionne)]);
//        private bool CanSupprimerMarque(object obj) => MarqueSelectionnee != null;

//        private void AjouterMarque(object obj)
//        {
//            var marque = new Marque
//            {
//                Nom = NomSelectionne,
//                Actif = ActifSelectionne
//            };

//            _repository.Add(marque);
//            Marques.Add(marque);

//            NomSelectionne = string.Empty;
//            ActifSelectionne = true;
//        }

//        private void ModifierMarque(object obj)
//        {
//            if (MarqueSelectionnee == null) return;

//            MarqueSelectionnee.Nom = NomSelectionne;
//            MarqueSelectionnee.Actif = ActifSelectionne;

//            _repository.Update(MarqueSelectionnee);

//            // ⚡ Rafraîchir le DataGrid
//            CollectionViewSource.GetDefaultView(Marques).Refresh();
//        }

//        private void SupprimerMarque(object obj)
//        {
//            if (MarqueSelectionnee == null) return;

//            _repository.Delete(MarqueSelectionnee);
//            Marques.Remove(MarqueSelectionnee);

//            // ⚡ Rafraîchir le DataGrid
//            CollectionViewSource.GetDefaultView(Marques).Refresh();

//            MarqueSelectionnee = null;
//            NomSelectionne = string.Empty;
//            ActifSelectionne = true;
//        }
//    }
//}



//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Windows.Input;
//using Stock741.Commands;
//using Stock741.Models;
//using Stock741.Repositories;

//namespace Stock741.ViewModels
//{
//    public class MarqueViewModel : BaseViewModel, IDataErrorInfo
//    {
//        private readonly MarqueRepository _repository;

//        public ObservableCollection<Marque> Marques { get; set; }

//        private string _nomSelectionne;
//        public string NomSelectionne
//        {
//            get => _nomSelectionne;
//            set { _nomSelectionne = value; OnPropertyChanged(); }
//        }

//        private bool _actifSelectionne = true;
//        public bool ActifSelectionne
//        {
//            get => _actifSelectionne;
//            set { _actifSelectionne = value; OnPropertyChanged(); }
//        }

//        private Marque _marqueSelectionnee;
//        public Marque MarqueSelectionnee
//        {
//            get => _marqueSelectionnee;
//            set
//            {
//                _marqueSelectionnee = value;
//                OnPropertyChanged();
//                if (value != null)
//                {
//                    NomSelectionne = value.Nom;
//                    ActifSelectionne = value.Actif;
//                }
//            }
//        }

//        public ICommand AjouterMarqueCommand { get; }
//        public ICommand ModifierMarqueCommand { get; }
//        public ICommand SupprimerMarqueCommand { get; }

//        public MarqueViewModel(MarqueRepository repository)
//        {
//            _repository = repository;
//            Marques = new ObservableCollection<Marque>(_repository.GetAll());

//            AjouterMarqueCommand = new RelayCommand(AjouterMarque, CanAjouterMarque);
//            ModifierMarqueCommand = new RelayCommand(ModifierMarque, CanModifierMarque);
//            SupprimerMarqueCommand = new RelayCommand(SupprimerMarque, CanSupprimerMarque);
//        }

//        // ----------------------
//        // Validation IDataErrorInfo
//        // ----------------------
//        public string this[string columnName]
//        {
//            get
//            {
//                if (columnName == nameof(NomSelectionne))
//                {
//                    if (string.IsNullOrWhiteSpace(NomSelectionne))
//                        return "Nom obligatoire";

//                    // Vérifie si le nom existe déjà (sauf pour la marque sélectionnée en modification)
//                    bool exists = Marques.Any(m =>
//                        m.Nom.ToLower() == NomSelectionne.ToLower() &&
//                        (MarqueSelectionnee == null || m.Id != MarqueSelectionnee.Id));

//                    if (exists)
//                        return "Nom déjà utilisé";
//                }

//                return null;
//            }
//        }

//        public string Error => null;

//        // ----------------------
//        // Commandes
//        // ----------------------
//        private bool CanAjouterMarque(object obj) => string.IsNullOrWhiteSpace(this[nameof(NomSelectionne)]) == true;
//        private bool CanModifierMarque(object obj) => MarqueSelectionnee != null && string.IsNullOrWhiteSpace(this[nameof(NomSelectionne)]) == true;
//        private bool CanSupprimerMarque(object obj) => MarqueSelectionnee != null;

//        private void AjouterMarque(object obj)
//        {
//            var marque = new Marque
//            {
//                Nom = NomSelectionne,
//                Actif = ActifSelectionne
//            };

//            _repository.Add(marque);
//            Marques.Add(marque);

//            NomSelectionne = string.Empty;
//            ActifSelectionne = true;
//        }

//        private void ModifierMarque(object obj)
//        {
//            if (MarqueSelectionnee == null) return;

//            MarqueSelectionnee.Nom = NomSelectionne;
//            MarqueSelectionnee.Actif = ActifSelectionne;

//            _repository.Update(MarqueSelectionnee);

//            int index = Marques.IndexOf(MarqueSelectionnee);
//            if (index >= 0)
//                Marques[index] = MarqueSelectionnee;
//        }

//        private void SupprimerMarque(object obj)
//        {
//            if (MarqueSelectionnee == null) return;

//            _repository.Delete(MarqueSelectionnee);
//            Marques.Remove(MarqueSelectionnee);

//            MarqueSelectionnee = null;
//            NomSelectionne = string.Empty;
//            ActifSelectionne = true;
//        }
//    }
//}

//using System.Collections.ObjectModel;
//using System.Windows.Input;
//using Stock741.Commands;
//using Stock741.Models;
//using Stock741.Repositories;

//namespace Stock741.ViewModels
//{
//    public class MarqueViewModel : BaseViewModel
//    {
//        private readonly MarqueRepository _repository;

//        public ObservableCollection<Marque> Marques { get; set; }

//        private string _nomSelectionne;
//        public string NomSelectionne
//        {
//            get => _nomSelectionne;
//            set { _nomSelectionne = value; OnPropertyChanged(); }
//        }

//        private bool _actifSelectionne = true;
//        public bool ActifSelectionne
//        {
//            get => _actifSelectionne;
//            set { _actifSelectionne = value; OnPropertyChanged(); }
//        }

//        private Marque _marqueSelectionnee;
//        public Marque MarqueSelectionnee
//        {
//            get => _marqueSelectionnee;
//            set
//            {
//                _marqueSelectionnee = value;
//                OnPropertyChanged();

//                if (value != null)
//                {
//                    NomSelectionne = value.Nom;
//                    ActifSelectionne = value.Actif;
//                }
//            }
//        }

//        public ICommand AjouterMarqueCommand { get; }
//        public ICommand ModifierMarqueCommand { get; }
//        public ICommand SupprimerMarqueCommand { get; }

//        public MarqueViewModel(MarqueRepository repository)
//        {
//            _repository = repository;
//            Marques = new ObservableCollection<Marque>(_repository.GetAll());

//            AjouterMarqueCommand = new RelayCommand(AjouterMarque, CanAjouterMarque);
//            ModifierMarqueCommand = new RelayCommand(ModifierMarque, CanModifierMarque);
//            SupprimerMarqueCommand = new RelayCommand(SupprimerMarque, CanSupprimerMarque);
//        }

//        private bool CanAjouterMarque(object obj) => !string.IsNullOrWhiteSpace(NomSelectionne);
//        private bool CanModifierMarque(object obj) => MarqueSelectionnee != null;
//        private bool CanSupprimerMarque(object obj) => MarqueSelectionnee != null;

//        private void AjouterMarque(object obj)
//        {
//            var marque = new Marque
//            {
//                Nom = NomSelectionne,
//                Actif = ActifSelectionne
//            };

//            _repository.Add(marque);
//            Marques.Add(marque);

//            NomSelectionne = string.Empty;
//            ActifSelectionne = true;
//        }

//        private void ModifierMarque(object obj)
//        {
//            if (MarqueSelectionnee == null) return;

//            MarqueSelectionnee.Nom = NomSelectionne;
//            MarqueSelectionnee.Actif = ActifSelectionne;

//            _repository.Update(MarqueSelectionnee);

//            // Refresh ObservableCollection si nécessaire
//            int index = Marques.IndexOf(MarqueSelectionnee);
//            if (index >= 0)
//            {
//                Marques[index] = MarqueSelectionnee;
//            }
//        }

//        private void SupprimerMarque(object obj)
//        {
//            if (MarqueSelectionnee == null) return;

//            _repository.Delete(MarqueSelectionnee);
//            Marques.Remove(MarqueSelectionnee);

//            MarqueSelectionnee = null;
//            NomSelectionne = string.Empty;
//            ActifSelectionne = true;
//        }
//    }
//}

//using System.Collections.ObjectModel;
//using System.Windows.Input;
//using Stock741.Commands;
//using Stock741.Models;
//using Stock741.Repositories;

//namespace Stock741.ViewModels
//{
//    public class MarqueViewModel : BaseViewModel
//    {
//        private readonly MarqueRepository _repository;

//        public ObservableCollection<Marque> Marques { get; set; }

//        // Pour l'ajout ou la modification
//        private string _nomSelectionne;
//        public string NomSelectionne
//        {
//            get => _nomSelectionne;
//            set { _nomSelectionne = value; OnPropertyChanged(); }
//        }

//        private bool _actifSelectionne = true;
//        public bool ActifSelectionne
//        {
//            get => _actifSelectionne;
//            set { _actifSelectionne = value; OnPropertyChanged(); }
//        }

//        private Marque _marqueSelectionnee;
//        public Marque MarqueSelectionnee
//        {
//            get => _marqueSelectionnee;
//            set
//            {
//                _marqueSelectionnee = value;
//                OnPropertyChanged();

//                if (value != null)
//                {
//                    NomSelectionne = value.Nom;
//                    ActifSelectionne = value.Actif;
//                }
//            }
//        }

//        public ICommand AjouterMarqueCommand { get; }
//        public ICommand ModifierMarqueCommand { get; }

//        public MarqueViewModel(MarqueRepository repository)
//        {
//            _repository = repository;
//            Marques = new ObservableCollection<Marque>(_repository.GetAll());

//            AjouterMarqueCommand = new RelayCommand(AjouterMarque, CanAjouterMarque);
//            ModifierMarqueCommand = new RelayCommand(ModifierMarque, CanModifierMarque);
//        }

//        private bool CanAjouterMarque(object obj) => !string.IsNullOrWhiteSpace(NomSelectionne);

//        private void AjouterMarque(object obj)
//        {
//            var marque = new Marque
//            {
//                Nom = NomSelectionne,
//                Actif = ActifSelectionne
//            };

//            _repository.Add(marque);
//            Marques.Add(marque);

//            NomSelectionne = string.Empty;
//            ActifSelectionne = true;
//        }

//        private bool CanModifierMarque(object obj) => MarqueSelectionnee != null;

//        private void ModifierMarque(object obj)
//        {
//            if (MarqueSelectionnee == null) return;

//            MarqueSelectionnee.Nom = NomSelectionne;
//            MarqueSelectionnee.Actif = ActifSelectionne;

//            _repository.Update(MarqueSelectionnee);

//            // Refresh ObservableCollection si nécessaire
//            int index = Marques.IndexOf(MarqueSelectionnee);
//            if (index >= 0)
//            {
//                Marques[index] = MarqueSelectionnee;
//            }
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
//    public class MarqueViewModel : BaseViewModel
//    {
//        private readonly MarqueRepository _repository;

//        public ObservableCollection<Marque> Marques { get; set; }

//        private string _nom;
//        public string Nom
//        {
//            get => _nom;
//            set { _nom = value; OnPropertyChanged(); }
//        }

//        private bool _actif = true;
//        public bool Actif
//        {
//            get => _actif;
//            set { _actif = value; OnPropertyChanged(); }
//        }

//        private Marque _marqueSelectionnee;
//        public Marque MarqueSelectionnee
//        {
//            get => _marqueSelectionnee;
//            set
//            {
//                _marqueSelectionnee = value;
//                OnPropertyChanged();
//                if (_marqueSelectionnee != null)
//                {
//                    Nom = _marqueSelectionnee.Nom;
//                    Actif = _marqueSelectionnee.Actif;
//                }
//            }
//        }

//        public ICommand AjouterMarqueCommand { get; }
//        public ICommand ModifierMarqueCommand { get; }
//        public ICommand SupprimerMarqueCommand { get; }

//        public MarqueViewModel(MarqueRepository repository)
//        {
//            _repository = repository;
//            Marques = new ObservableCollection<Marque>(_repository.GetAll());

//            AjouterMarqueCommand = new RelayCommand(_ => Ajouter());
//            ModifierMarqueCommand = new RelayCommand(_ => Modifier());
//            SupprimerMarqueCommand = new RelayCommand(_ => Supprimer());
//        }

//        private void Ajouter()
//        {
//            if (string.IsNullOrWhiteSpace(Nom)) return;

//            var marque = new Marque { Nom = Nom, Actif = Actif };
//            _repository.Add(marque);
//            Marques.Add(marque);

//            Nom = "";
//            Actif = true;
//        }

//        private void Modifier()
//        {
//            if (MarqueSelectionnee == null) return;

//            MarqueSelectionnee.Nom = Nom;
//            MarqueSelectionnee.Actif = Actif;

//            _repository.Update(MarqueSelectionnee);
//        }

//        private void Supprimer()
//        {
//            if (MarqueSelectionnee == null) return;

//            _repository.Delete(MarqueSelectionnee);
//            Marques.Remove(MarqueSelectionnee);
//        }
//    }
//}
