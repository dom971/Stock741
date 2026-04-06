using System.Collections.ObjectModel;
using System.Windows.Data;
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

        public StatutViewModel(StatutRepository repository)
        {
            _repository = repository;
            Statuts = new ObservableCollection<Statut>(_repository.GetAll());

            AjouterStatutCommand = new RelayCommand(AjouterStatut);
            ModifierStatutCommand = new RelayCommand(ModifierStatut);
            SupprimerStatutCommand = new RelayCommand(SupprimerStatut);
        }

        public void Rafraichir()
        {
            Statuts.Clear();
            foreach (var m in _repository.GetAll())
                Statuts.Add(m);
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
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

        private void AjouterStatut(object obj)
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

            var statut = new Statut
            {
                Nom = NomSelectionne,
                Type = TypeSelectionne
            };

            try
            {
                _repository.Add(statut);
                //Statuts.Add(statut);
                Rafraichir();
                NomSelectionne = string.Empty;
                TypeSelectionne = null;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private void ModifierStatut(object obj)
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
                _repository.Update(StatutSelectionne);
                //CollectionViewSource.GetDefaultView(Statuts).Refresh();
                Rafraichir();
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                StatutSelectionne.Nom = ancienNom;
                StatutSelectionne.Type = ancienType;
                ErreurGlobale = ex.Message;
            }
        }

        private void SupprimerStatut(object obj)
        {
            if (StatutSelectionne == null) return;

            try
            {
                _repository.Delete(StatutSelectionne);
                //Statuts.Remove(StatutSelectionne);
                Rafraichir();
                StatutSelectionne = null;
                NomSelectionne = string.Empty;
                TypeSelectionne = null;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
                StatutSelectionne = null;
                NomSelectionne = string.Empty;
                TypeSelectionne = null;
            }
        }
    }
}