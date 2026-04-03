using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class MaterielViewModel : BaseViewModel
    {
        private readonly MaterielRepository _repository;

        public ObservableCollection<Materiel> Materiels { get; set; }

        private string _nomSelectionne;
        public string NomSelectionne
        {
            get => _nomSelectionne;
            set { _nomSelectionne = value; OnPropertyChanged(); ValidateNom(); }
        }

        private bool _actifSelectionne = true;
        public bool ActifSelectionne
        {
            get => _actifSelectionne;
            set { _actifSelectionne = value; OnPropertyChanged(); }
        }

        private Materiel _materielSelectionne;
        public Materiel MaterielSelectionne
        {
            get => _materielSelectionne;
            set
            {
                _materielSelectionne = value;
                OnPropertyChanged();
                if (value != null)
                {
                    NomSelectionne = value.Nom;
                    ActifSelectionne = value.Actif;
                }                   
            }
        }

        private string _erreurNom;
        public string ErreurNom
        {
            get => _erreurNom;
            set { _erreurNom = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        public bool HasErreur => !string.IsNullOrWhiteSpace(ErreurNom);

        private string _erreurGlobale;
        public string ErreurGlobale
        {
            get => _erreurGlobale;
            set { _erreurGlobale = value; OnPropertyChanged(); }
        }

        public ICommand AjouterMaterielCommand { get; }
        public ICommand ModifierMaterielCommand { get; }
        public ICommand SupprimerMaterielCommand { get; }

        public MaterielViewModel(MaterielRepository repository)
        {
            _repository = repository;
            Materiels = new ObservableCollection<Materiel>(_repository.GetAll());

            AjouterMaterielCommand = new RelayCommand(AjouterMateriel);
            ModifierMaterielCommand = new RelayCommand(ModifierMateriel);
            SupprimerMaterielCommand = new RelayCommand(SupprimerMateriel);
        }

        private void ValidateNom()
        {
            if (string.IsNullOrWhiteSpace(NomSelectionne))
                ErreurNom = "Nom obligatoire";
            else if (Materiels.Any(m => m.Nom.ToLower() == NomSelectionne.ToLower() &&
                                        (MaterielSelectionne == null || m.Id != MaterielSelectionne.Id)))
                ErreurNom = "Nom déjà utilisé";
            else
                ErreurNom = string.Empty;
        }

        private void AjouterMateriel(object obj)
        {
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var materiel = new Materiel { Nom = NomSelectionne, Actif = ActifSelectionne };

            try
            {
                _repository.Add(materiel);
                Materiels.Add(materiel);
                NomSelectionne = string.Empty;
                ActifSelectionne = true;
                ErreurGlobale = string.Empty;
            }

            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private void ModifierMateriel(object obj)
        {
            if (MaterielSelectionne == null) return;
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var ancienNom = MaterielSelectionne.Nom;
            var ancienActif = MaterielSelectionne.Actif;

            MaterielSelectionne.Nom = NomSelectionne;
            MaterielSelectionne.Actif = ActifSelectionne;

            try
            {
                _repository.Update(MaterielSelectionne);
                CollectionViewSource.GetDefaultView(Materiels).Refresh();
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                MaterielSelectionne.Nom = ancienNom;
                MaterielSelectionne.Actif = ancienActif;
                ErreurGlobale = ex.Message;
            }

        }

        private void SupprimerMateriel(object obj)
        {
            if (MaterielSelectionne == null) return;

            try
            {
                _repository.Delete(MaterielSelectionne);
                Materiels.Remove(MaterielSelectionne);
                MaterielSelectionne = null;
                NomSelectionne = string.Empty;
                ActifSelectionne = true;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
                MaterielSelectionne = null;
                NomSelectionne = string.Empty;
                ActifSelectionne = true;
            }
        }
    }
}
