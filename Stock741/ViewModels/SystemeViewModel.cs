
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class SystemeViewModel : BaseViewModel
    {
        private readonly SystemeRepository _repository;

        public ObservableCollection<Systeme> Systemes { get; set; }

        private string _nomSelectionne;
        public string NomSelectionne
        {
            get => _nomSelectionne;
            set { _nomSelectionne = value; OnPropertyChanged(); ValidateNom(); }
        }

        private Systeme _systemeSelectionne;
        public Systeme SystemeSelectionne
        {
            get => _systemeSelectionne;
            set
            {
                _systemeSelectionne = value;
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

        public ICommand AjouterSystemeCommand { get; }
        public ICommand ModifierSystemeCommand { get; }
        public ICommand SupprimerSystemeCommand { get; }
        public ICommand ActualiserCommand { get; }

        public SystemeViewModel(SystemeRepository repository)
        {
            _repository = repository;
            Systemes = new ObservableCollection<Systeme>();

            AjouterSystemeCommand = new AsyncRelayCommand(AjouterSysteme);
            ModifierSystemeCommand = new AsyncRelayCommand(ModifierSysteme);
            SupprimerSystemeCommand = new AsyncRelayCommand(SupprimerSysteme);
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
            else if (Systemes.Any(s => s.Nom.ToLower() == NomSelectionne.ToLower() &&
                                       (SystemeSelectionne == null || s.Id != SystemeSelectionne.Id)))
                ErreurNom = "Nom déjà utilisé";
            else
                ErreurNom = string.Empty;
        }

        public async Task Rafraichir()
        {
            var liste = await _repository.GetAll();
            App.Current.Dispatcher.Invoke(() =>
            {
                Systemes.Clear();
                foreach (var s in liste)
                    Systemes.Add(s);
            });
        }

        public void EffacerChamps()
        {
            SystemeSelectionne = null;
            NomSelectionne = string.Empty;
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
            ErreurNom = string.Empty;
        }

        private async Task AjouterSysteme(object obj)
        {
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var systeme = new Systeme { Nom = NomSelectionne };

            try
            {
                await _repository.Add(systeme);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task ModifierSysteme(object obj)
        {
            if (SystemeSelectionne == null) return;
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var ancienNom = SystemeSelectionne.Nom;
            SystemeSelectionne.Nom = NomSelectionne;

            try
            {
                await _repository.Update(SystemeSelectionne);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                SystemeSelectionne.Nom = ancienNom;
                ErreurGlobale = ex.Message;
            }
        }

        private async Task SupprimerSysteme(object obj)
        {
            if (SystemeSelectionne == null) return;

            try
            {
                await _repository.Delete(SystemeSelectionne);
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