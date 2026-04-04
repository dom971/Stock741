using System.Collections.ObjectModel;
using System.Windows.Data;
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
            Fournisseurs = new ObservableCollection<Fournisseur>(_repository.GetAll());

            AjouterFournisseurCommand = new RelayCommand(AjouterFournisseur);
            ModifierFournisseurCommand = new RelayCommand(ModifierFournisseur);
            SupprimerFournisseurCommand = new RelayCommand(SupprimerFournisseur);
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

        private void AjouterFournisseur(object obj)
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
                _repository.Add(fournisseur);
                Fournisseurs.Add(fournisseur);
                NomSelectionne = string.Empty;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private void ModifierFournisseur(object obj)
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
                _repository.Update(FournisseurSelectionne);
                CollectionViewSource.GetDefaultView(Fournisseurs).Refresh();
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                FournisseurSelectionne.Nom = ancienNom;
                ErreurGlobale = ex.Message;
            }
        }

        private void SupprimerFournisseur(object obj)
        {
            if (FournisseurSelectionne == null) return;

            try
            {
                _repository.Delete(FournisseurSelectionne);
                Fournisseurs.Remove(FournisseurSelectionne);
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