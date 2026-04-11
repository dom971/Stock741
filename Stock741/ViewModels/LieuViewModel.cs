using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class LieuViewModel : BaseViewModel
    {
        private readonly LieuRepository _repository;

        public ObservableCollection<Lieu> Lieux { get; set; }

        private string _nomSelectionne;
        public string NomSelectionne
        {
            get => _nomSelectionne;
            set { _nomSelectionne = value; OnPropertyChanged(); ValidateNom(); }
        }

        private Lieu _lieuSelectionne;
        public Lieu LieuSelectionne
        {
            get => _lieuSelectionne;
            set
            {
                _lieuSelectionne = value;
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

        public ICommand AjouterLieuCommand { get; }
        public ICommand ModifierLieuCommand { get; }
        public ICommand SupprimerLieuCommand { get; }
        public ICommand ActualiserCommand { get; }

        public LieuViewModel(LieuRepository repository)
        {
            _repository = repository;
            Lieux = new ObservableCollection<Lieu>();

            AjouterLieuCommand = new AsyncRelayCommand(AjouterLieu);
            ModifierLieuCommand = new AsyncRelayCommand(ModifierLieu);
            SupprimerLieuCommand = new AsyncRelayCommand(SupprimerLieu);
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
            else if (Lieux.Any(l => l.Nom.ToLower() == NomSelectionne.ToLower() &&
                                    (LieuSelectionne == null || l.Id != LieuSelectionne.Id)))
                ErreurNom = "Nom déjà utilisé";
            else
                ErreurNom = string.Empty;
        }

        public async Task Rafraichir()
        {
            var liste = await _repository.GetAll();
            App.Current.Dispatcher.Invoke(() =>
            {
                Lieux.Clear();
                foreach (var l in liste)
                    Lieux.Add(l);
            });
        }

        public void EffacerChamps()
        {
            LieuSelectionne = null;
            NomSelectionne = string.Empty;
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
            ErreurNom = string.Empty;
        }

        private async Task AjouterLieu(object obj)
        {
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var lieu = new Lieu { Nom = NomSelectionne };

            try
            {
                await _repository.Add(lieu);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task ModifierLieu(object obj)
        {
            if (LieuSelectionne == null) return;
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var ancienNom = LieuSelectionne.Nom;
            LieuSelectionne.Nom = NomSelectionne;

            try
            {
                await _repository.Update(LieuSelectionne);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                LieuSelectionne.Nom = ancienNom;
                ErreurGlobale = ex.Message;
            }
        }

        private async Task SupprimerLieu(object obj)
        {
            if (LieuSelectionne == null) return;

            try
            {
                await _repository.Delete(LieuSelectionne);
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