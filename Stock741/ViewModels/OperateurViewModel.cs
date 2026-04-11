using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class OperateurViewModel : BaseViewModel
    {
        private readonly OperateurRepository _repository;

        public ObservableCollection<Operateur> Operateurs { get; set; }

        private string _nomSelectionne;
        public string NomSelectionne
        {
            get => _nomSelectionne;
            set { _nomSelectionne = value; OnPropertyChanged(); ValidateNom(); }
        }

        private Operateur _operateurSelectionne;
        public Operateur OperateurSelectionne
        {
            get => _operateurSelectionne;
            set
            {
                _operateurSelectionne = value;
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

        public ICommand AjouterOperateurCommand { get; }
        public ICommand ModifierOperateurCommand { get; }
        public ICommand SupprimerOperateurCommand { get; }
        public ICommand ActualiserCommand { get; }

        public OperateurViewModel(OperateurRepository repository)
        {
            _repository = repository;
            Operateurs = new ObservableCollection<Operateur>();

            AjouterOperateurCommand = new AsyncRelayCommand(AjouterOperateur);
            ModifierOperateurCommand = new AsyncRelayCommand(ModifierOperateur);
            SupprimerOperateurCommand = new AsyncRelayCommand(SupprimerOperateur);
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
            else if (Operateurs.Any(o => o.Nom.ToLower() == NomSelectionne.ToLower() &&
                                         (OperateurSelectionne == null || o.Id != OperateurSelectionne.Id)))
                ErreurNom = "Nom déjà utilisé";
            else
                ErreurNom = string.Empty;
        }

        public async Task Rafraichir()
        {
            var liste = await _repository.GetAll();
            App.Current.Dispatcher.Invoke(() =>
            {
                Operateurs.Clear();
                foreach (var o in liste)
                    Operateurs.Add(o);
            });
        }

        public void EffacerChamps()
        {
            OperateurSelectionne = null;
            NomSelectionne = string.Empty;
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
            ErreurNom = string.Empty;
        }

        private async Task AjouterOperateur(object obj)
        {
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var operateur = new Operateur { Nom = NomSelectionne };

            try
            {
                await _repository.Add(operateur);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task ModifierOperateur(object obj)
        {
            if (OperateurSelectionne == null) return;
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var ancienNom = OperateurSelectionne.Nom;
            OperateurSelectionne.Nom = NomSelectionne;

            try
            {
                await _repository.Update(OperateurSelectionne);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                OperateurSelectionne.Nom = ancienNom;
                ErreurGlobale = ex.Message;
            }
        }

        private async Task SupprimerOperateur(object obj)
        {
            if (OperateurSelectionne == null) return;

            try
            {
                await _repository.Delete(OperateurSelectionne);
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