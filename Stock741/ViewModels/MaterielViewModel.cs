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
        private readonly FicheRepository _ficheRepository;

        public ObservableCollection<Materiel> Materiels { get; set; }
        public ObservableCollection<Fiche> Fiches { get; set; }

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

        private Fiche _ficheSelectionnee;
        public Fiche FicheSelectionnee
        {
            get => _ficheSelectionnee;
            set { _ficheSelectionnee = value; OnPropertyChanged(); }
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
                    FicheSelectionnee = Fiches.FirstOrDefault(f => f.Id == value.FicheId);
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

        public MaterielViewModel(MaterielRepository repository, FicheRepository ficheRepository)
        {
            _repository = repository;
            _ficheRepository = ficheRepository;
            Materiels = new ObservableCollection<Materiel>(_repository.GetAll());
            Fiches = new ObservableCollection<Fiche>(_ficheRepository.GetAll());

            AjouterMaterielCommand = new RelayCommand(AjouterMateriel);
            ModifierMaterielCommand = new RelayCommand(ModifierMateriel);
            SupprimerMaterielCommand = new RelayCommand(SupprimerMateriel);
        }

        public void Rafraichir()
        {
            Materiels.Clear();
            foreach (var m in _repository.GetAll())
                Materiels.Add(m);
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
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

            if (FicheSelectionnee == null)
            {
                ErreurGlobale = "Veuillez sélectionner une fiche.";
                return;
            }

            var materiel = new Materiel
            {
                Nom = NomSelectionne,
                Actif = ActifSelectionne,
                FicheId = FicheSelectionnee.Id
            };

            try
            {
                _repository.Add(materiel);
                materiel.Fiche = FicheSelectionnee;
                //Materiels.Add(materiel);
                Rafraichir();
                NomSelectionne = string.Empty;
                ActifSelectionne = true;
                FicheSelectionnee = null;
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

            if (FicheSelectionnee == null)
            {
                ErreurGlobale = "Veuillez sélectionner une fiche.";
                return;
            }

            var ancienNom = MaterielSelectionne.Nom;
            var ancienActif = MaterielSelectionne.Actif;
            var ancienFicheId = MaterielSelectionne.FicheId;
            var ancienneFiche = MaterielSelectionne.Fiche;

            MaterielSelectionne.Nom = NomSelectionne;
            MaterielSelectionne.Actif = ActifSelectionne;
            MaterielSelectionne.FicheId = FicheSelectionnee.Id;
            MaterielSelectionne.Fiche = FicheSelectionnee;

            try
            {
                _repository.Update(MaterielSelectionne);
                //CollectionViewSource.GetDefaultView(Materiels).Refresh();
                Rafraichir();
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                MaterielSelectionne.Nom = ancienNom;
                MaterielSelectionne.Actif = ancienActif;
                MaterielSelectionne.FicheId = ancienFicheId;
                MaterielSelectionne.Fiche = ancienneFiche;
                ErreurGlobale = ex.Message;
            }
        }

        private void SupprimerMateriel(object obj)
        {
            if (MaterielSelectionne == null) return;

            try
            {
                _repository.Delete(MaterielSelectionne);
                //Materiels.Remove(MaterielSelectionne);
                Rafraichir();
                MaterielSelectionne = null;
                NomSelectionne = string.Empty;
                ActifSelectionne = true;
                FicheSelectionnee = null;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
                MaterielSelectionne = null;
                NomSelectionne = string.Empty;
                ActifSelectionne = true;
                FicheSelectionnee = null;
            }
        }
    }
}