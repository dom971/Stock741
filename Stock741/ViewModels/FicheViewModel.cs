using System.Collections.ObjectModel;
using System.Windows.Data;
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
            Fiches = new ObservableCollection<Fiche>(_repository.GetAll());

            AjouterFicheCommand = new RelayCommand(AjouterFiche);
            ModifierFicheCommand = new RelayCommand(ModifierFiche);
            SupprimerFicheCommand = new RelayCommand(SupprimerFiche);
        }

        public void Rafraichir()
        {
            Fiches.Clear();
            foreach (var m in _repository.GetAll())
                Fiches.Add(m);
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
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

        private void AjouterFiche(object obj)
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
                _repository.Add(fiche);
                //Fiches.Add(fiche);
                Rafraichir();
                NomSelectionne = string.Empty;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private void ModifierFiche(object obj)
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
                _repository.Update(FicheSelectionnee);
                //CollectionViewSource.GetDefaultView(Fiches).Refresh();
                Rafraichir();
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                FicheSelectionnee.Nom = ancienNom;
                ErreurGlobale = ex.Message;
            }
        }

        private void SupprimerFiche(object obj)
        {
            if (FicheSelectionnee == null) return;

            try
            {
                _repository.Delete(FicheSelectionnee);
                //Fiches.Remove(FicheSelectionnee);
                Rafraichir();
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
