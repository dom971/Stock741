using System.Collections.ObjectModel;
using System.Windows.Data;
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

        public LieuViewModel(LieuRepository repository)
        {
            _repository = repository;
            Lieux = new ObservableCollection<Lieu>(_repository.GetAll());

            AjouterLieuCommand = new RelayCommand(AjouterLieu);
            ModifierLieuCommand = new RelayCommand(ModifierLieu);
            SupprimerLieuCommand = new RelayCommand(SupprimerLieu);
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

        private void AjouterLieu(object obj)
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
                _repository.Add(lieu);
                Lieux.Add(lieu);
                NomSelectionne = string.Empty;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private void ModifierLieu(object obj)
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
                _repository.Update(LieuSelectionne);
                CollectionViewSource.GetDefaultView(Lieux).Refresh();
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                LieuSelectionne.Nom = ancienNom;
                ErreurGlobale = ex.Message;
            }
        }

        private void SupprimerLieu(object obj)
        {
            if (LieuSelectionne == null) return;

            try
            {
                _repository.Delete(LieuSelectionne);
                Lieux.Remove(LieuSelectionne);
                LieuSelectionne = null;
                NomSelectionne = string.Empty;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
                LieuSelectionne = null;
                NomSelectionne = string.Empty;
            }
        }
    }
}
