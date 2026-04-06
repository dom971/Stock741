using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class MarqueViewModel : BaseViewModel
    {
        private readonly MarqueRepository _repository;

        public ObservableCollection<Marque> Marques { get; set; }

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

        private Marque _marqueSelectionnee;
        public Marque MarqueSelectionnee
        {
            get => _marqueSelectionnee;
            set
            {
                _marqueSelectionnee = value;
                OnPropertyChanged();
                if (value != null)
                {
                    NomSelectionne = value.Nom;
                    ActifSelectionne = value.Actif;
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

        public ICommand AjouterMarqueCommand { get; }
        public ICommand ModifierMarqueCommand { get; }
        public ICommand SupprimerMarqueCommand { get; }

        public MarqueViewModel(MarqueRepository repository)
        {
            _repository = repository;
            Marques = new ObservableCollection<Marque>(_repository.GetAll());

            AjouterMarqueCommand = new RelayCommand(AjouterMarque);
            ModifierMarqueCommand = new RelayCommand(ModifierMarque);
            SupprimerMarqueCommand = new RelayCommand(SupprimerMarque);
        }

        public void Rafraichir()
        {
            Marques.Clear();
            foreach (var m in _repository.GetAll())
                Marques.Add(m);
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
        }

        private void ValidateNom()
        {
            if (string.IsNullOrWhiteSpace(NomSelectionne))
                ErreurNom = "Nom obligatoire";
            else if (Marques.Any(m => m.Nom.ToLower() == NomSelectionne.ToLower() &&
                                      (MarqueSelectionnee == null || m.Id != MarqueSelectionnee.Id)))
                ErreurNom = "Nom déjà utilisé";
            else
                ErreurNom = string.Empty;
        }

        private void AjouterMarque(object obj)
        {
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var marque = new Marque { Nom = NomSelectionne, Actif = ActifSelectionne };

            try
            {
                _repository.Add(marque);
                //Marques.Add(marque);
                Rafraichir();
                NomSelectionne = string.Empty;
                ActifSelectionne = true;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private void ModifierMarque(object obj)
        {
            if (MarqueSelectionnee == null) return;
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            var ancienNom = MarqueSelectionnee.Nom;
            var ancienActif = MarqueSelectionnee.Actif;

            MarqueSelectionnee.Nom = NomSelectionne;
            MarqueSelectionnee.Actif = ActifSelectionne;

            try
            {
                _repository.Update(MarqueSelectionnee);                
                Rafraichir();
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                MarqueSelectionnee.Nom = ancienNom;
                MarqueSelectionnee.Actif = ancienActif;
                ErreurGlobale = ex.Message;
            }
        }

        private void SupprimerMarque(object obj)
        {
            if (MarqueSelectionnee == null) return;

            try
            {
                _repository.Delete(MarqueSelectionnee);                
                Rafraichir();
                MarqueSelectionnee = null;
                NomSelectionne = string.Empty;
                ActifSelectionne = true;
                ErreurGlobale = string.Empty;
            }            
            catch (Exception ex)
            {                
                ErreurGlobale = ex.Message;
                ErreurNom = string.Empty;
                MarqueSelectionnee = null;
                NomSelectionne = string.Empty;
                ActifSelectionne = true;
            }
        }
    }
}