using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class FournisseurViewModel : BaseViewModel
    {
        private readonly FournisseurRepository _repository;

        public ObservableCollection<Fournisseur> Fournisseurs { get; set; }

        private string _nomSelectionne;
        public string NomSelectionne
        {
            get => _nomSelectionne;
            set { _nomSelectionne = value; OnPropertyChanged(); ValidateNom(); }
        }

        private Fournisseur _fournisseurSelectionne;
        public Fournisseur FournisseurSelectionne
        {
            get => _fournisseurSelectionne;
            set
            {
                _fournisseurSelectionne = value;
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

        public ICommand AjouterFournisseurCommand { get; }
        public ICommand ModifierFournisseurCommand { get; }
        public ICommand SupprimerFournisseurCommand { get; }

        public FournisseurViewModel(FournisseurRepository repository)
        {
            _repository = repository;
            Fournisseurs = new ObservableCollection<Fournisseur>();

            AjouterFournisseurCommand = new AsyncRelayCommand(AjouterFournisseur);
            ModifierFournisseurCommand = new AsyncRelayCommand(ModifierFournisseur);
            SupprimerFournisseurCommand = new AsyncRelayCommand(SupprimerFournisseur);
        }

        private void ValidateNom()
        {
            if (string.IsNullOrWhiteSpace(NomSelectionne))
                ErreurNom = "Nom obligatoire";
            else if (Fournisseurs.Any(f => f.Nom.ToLower() == NomSelectionne.ToLower() &&
                                           (FournisseurSelectionne == null || f.Id != FournisseurSelectionne.Id)))
                ErreurNom = "Nom déjà utilisé";
            else
                ErreurNom = string.Empty;
        }

        public async Task Rafraichir()
        {
            var liste = await _repository.GetAll();
            App.Current.Dispatcher.Invoke(() =>
            {
                Fournisseurs.Clear();
                foreach (var f in liste)
                    Fournisseurs.Add(f);
            });
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
        }

        private async Task AjouterFournisseur(object obj)
        {
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var fournisseur = new Fournisseur { Nom = NomSelectionne };

            try
            {
                await _repository.Add(fournisseur);
                await Rafraichir();
                NomSelectionne = string.Empty;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task ModifierFournisseur(object obj)
        {
            if (FournisseurSelectionne == null) return;
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var ancienNom = FournisseurSelectionne.Nom;
            FournisseurSelectionne.Nom = NomSelectionne;

            try
            {
                await _repository.Update(FournisseurSelectionne);
                await Rafraichir();
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                FournisseurSelectionne.Nom = ancienNom;
                ErreurGlobale = ex.Message;
            }
        }

        private async Task SupprimerFournisseur(object obj)
        {
            if (FournisseurSelectionne == null) return;

            try
            {
                await _repository.Delete(FournisseurSelectionne);
                await Rafraichir();
                FournisseurSelectionne = null;
                NomSelectionne = string.Empty;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
                FournisseurSelectionne = null;
                NomSelectionne = string.Empty;
            }
        }
    }
}