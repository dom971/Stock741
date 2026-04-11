using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
        public ICommand ActualiserCommand { get; }

        public MaterielViewModel(MaterielRepository repository, FicheRepository ficheRepository)
        {
            _repository = repository;
            _ficheRepository = ficheRepository;
            Materiels = new ObservableCollection<Materiel>();
            Fiches = new ObservableCollection<Fiche>();

            AjouterMaterielCommand = new AsyncRelayCommand(AjouterMateriel);
            ModifierMaterielCommand = new AsyncRelayCommand(ModifierMateriel);
            SupprimerMaterielCommand = new AsyncRelayCommand(SupprimerMateriel);
            ActualiserCommand = new AsyncRelayCommand(async _ =>
            {
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            });
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

        public async Task Rafraichir()
        {
            var materiels = await _repository.GetAll();
            var fiches = await _ficheRepository.GetAll();
            App.Current.Dispatcher.Invoke(() =>
            {
                Materiels.Clear();
                foreach (var m in materiels)
                    Materiels.Add(m);

                Fiches.Clear();
                foreach (var f in fiches)
                    Fiches.Add(f);
            });
        }

        public void EffacerChamps()
        {
            MaterielSelectionne = null;
            NomSelectionne = string.Empty;
            ActifSelectionne = true;
            FicheSelectionnee = null;
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
            ErreurNom = string.Empty;
        }

        private async Task AjouterMateriel(object obj)
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
                await _repository.Add(materiel);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task ModifierMateriel(object obj)
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
                await _repository.Update(MaterielSelectionne);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
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

        private async Task SupprimerMateriel(object obj)
        {
            if (MaterielSelectionne == null) return;

            try
            {
                await _repository.Delete(MaterielSelectionne);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
                EffacerChamps();
            }
        }
    }
}