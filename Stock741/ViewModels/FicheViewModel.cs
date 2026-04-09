using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class FicheViewModel : BaseViewModel
    {
        private readonly FicheRepository _repository;

        public ObservableCollection<Fiche> Fiches { get; set; }

        private string _nomSelectionne;
        public string NomSelectionne
        {
            get => _nomSelectionne;
            set { _nomSelectionne = value; OnPropertyChanged(); ValidateNom(); }
        }

        private Fiche _ficheSelectionnee;
        public Fiche FicheSelectionnee
        {
            get => _ficheSelectionnee;
            set
            {
                _ficheSelectionnee = value;
                OnPropertyChanged();
                if (value != null)
                    NomSelectionne = value.Nom;
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

        public ICommand AjouterFicheCommand { get; }
        public ICommand ModifierFicheCommand { get; }
        public ICommand SupprimerFicheCommand { get; }

        public FicheViewModel(FicheRepository repository)
        {
            _repository = repository;
            Fiches = new ObservableCollection<Fiche>();

            AjouterFicheCommand = new AsyncRelayCommand(AjouterFiche);
            ModifierFicheCommand = new AsyncRelayCommand(ModifierFiche);
            SupprimerFicheCommand = new AsyncRelayCommand(SupprimerFiche);
        }

        private void ValidateNom()
        {
            if (string.IsNullOrWhiteSpace(NomSelectionne))
                ErreurNom = "Nom obligatoire";
            else if (Fiches.Any(f => f.Nom.ToLower() == NomSelectionne.ToLower() &&
                                     (FicheSelectionnee == null || f.Id != FicheSelectionnee.Id)))
                ErreurNom = "Nom déjà utilisé";
            else
                ErreurNom = string.Empty;
        }

        public async Task Rafraichir()
        {
            var liste = await _repository.GetAll();
            App.Current.Dispatcher.Invoke(() =>
            {
                Fiches.Clear();
                foreach (var f in liste)
                    Fiches.Add(f);
            });
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
        }

        private async Task AjouterFiche(object obj)
        {
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var fiche = new Fiche { Nom = NomSelectionne };

            try
            {
                await _repository.Add(fiche);
                await Rafraichir();
                NomSelectionne = string.Empty;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task ModifierFiche(object obj)
        {
            if (FicheSelectionnee == null) return;
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var ancienNom = FicheSelectionnee.Nom;
            FicheSelectionnee.Nom = NomSelectionne;

            try
            {
                await _repository.Update(FicheSelectionnee);
                await Rafraichir();
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                FicheSelectionnee.Nom = ancienNom;
                ErreurGlobale = ex.Message;
            }
        }

        private async Task SupprimerFiche(object obj)
        {
            if (FicheSelectionnee == null) return;

            try
            {
                await _repository.Delete(FicheSelectionnee);
                await Rafraichir();
                FicheSelectionnee = null;
                NomSelectionne = string.Empty;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
                FicheSelectionnee = null;
                NomSelectionne = string.Empty;
            }
        }
    }
}