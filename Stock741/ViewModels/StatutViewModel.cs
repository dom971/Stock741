using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class StatutViewModel : BaseViewModel
    {
        private readonly StatutRepository _repository;

        public ObservableCollection<Statut> Statuts { get; set; }
        public List<string> Types { get; } = new List<string> { "Affectation", "Retour" };

        private string _nomSelectionne;
        public string NomSelectionne
        {
            get => _nomSelectionne;
            set { _nomSelectionne = value; OnPropertyChanged(); ValidateNom(); }
        }

        private string _typeSelectionne;
        public string TypeSelectionne
        {
            get => _typeSelectionne;
            set { _typeSelectionne = value; OnPropertyChanged(); }
        }

        private Statut _statutSelectionne;
        public Statut StatutSelectionne
        {
            get => _statutSelectionne;
            set
            {
                _statutSelectionne = value;
                OnPropertyChanged();
                if (value != null)
                {
                    NomSelectionne = value.Nom;
                    TypeSelectionne = value.Type;
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

        public ICommand AjouterStatutCommand { get; }
        public ICommand ModifierStatutCommand { get; }
        public ICommand SupprimerStatutCommand { get; }
        public ICommand ActualiserCommand { get; }

        public StatutViewModel(StatutRepository repository)
        {
            _repository = repository;
            Statuts = new ObservableCollection<Statut>();

            AjouterStatutCommand = new AsyncRelayCommand(AjouterStatut);
            ModifierStatutCommand = new AsyncRelayCommand(ModifierStatut);
            SupprimerStatutCommand = new AsyncRelayCommand(SupprimerStatut);
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
            else if (Statuts.Any(s => s.Nom.ToLower() == NomSelectionne.ToLower() &&
                                      (StatutSelectionne == null || s.Id != StatutSelectionne.Id)))
                ErreurNom = "Nom déjà utilisé";
            else
                ErreurNom = string.Empty;
        }

        public async Task Rafraichir()
        {
            var liste = await _repository.GetAll();
            App.Current.Dispatcher.Invoke(() =>
            {
                Statuts.Clear();
                foreach (var s in liste)
                    Statuts.Add(s);
            });
        }

        public void EffacerChamps()
        {
            StatutSelectionne = null;
            NomSelectionne = string.Empty;
            TypeSelectionne = null;
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
            ErreurNom = string.Empty;
        }

        private async Task AjouterStatut(object obj)
        {
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            if (string.IsNullOrWhiteSpace(TypeSelectionne))
            {
                ErreurGlobale = "Veuillez sélectionner un type.";
                return;
            }

            var statut = new Statut { Nom = NomSelectionne, Type = TypeSelectionne };

            try
            {
                await _repository.Add(statut);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task ModifierStatut(object obj)
        {
            if (StatutSelectionne == null) return;
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            if (string.IsNullOrWhiteSpace(TypeSelectionne))
            {
                ErreurGlobale = "Veuillez sélectionner un type.";
                return;
            }

            var ancienNom = StatutSelectionne.Nom;
            var ancienType = StatutSelectionne.Type;

            StatutSelectionne.Nom = NomSelectionne;
            StatutSelectionne.Type = TypeSelectionne;

            try
            {
                await _repository.Update(StatutSelectionne);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                StatutSelectionne.Nom = ancienNom;
                StatutSelectionne.Type = ancienType;
                ErreurGlobale = ex.Message;
            }
        }

        private async Task SupprimerStatut(object obj)
        {
            if (StatutSelectionne == null) return;

            try
            {
                await _repository.Delete(StatutSelectionne);
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