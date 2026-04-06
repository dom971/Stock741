
using System.Collections.ObjectModel;
using System.Windows.Data;
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

        public OperateurViewModel(OperateurRepository repository)
        {
            _repository = repository;
            Operateurs = new ObservableCollection<Operateur>(_repository.GetAll());

            AjouterOperateurCommand = new RelayCommand(AjouterOperateur);
            ModifierOperateurCommand = new RelayCommand(ModifierOperateur);
            SupprimerOperateurCommand = new RelayCommand(SupprimerOperateur);
        }

        public void Rafraichir()
        {
            Operateurs.Clear();
            foreach (var m in _repository.GetAll())
                Operateurs.Add(m);
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
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

        private void AjouterOperateur(object obj)
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
                _repository.Add(operateur);
                //Operateurs.Add(operateur);
                Rafraichir();
                NomSelectionne = string.Empty;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private void ModifierOperateur(object obj)
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
                _repository.Update(OperateurSelectionne);
                //CollectionViewSource.GetDefaultView(Operateurs).Refresh();
                Rafraichir();
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                OperateurSelectionne.Nom = ancienNom;
                ErreurGlobale = ex.Message;
            }
        }

        private void SupprimerOperateur(object obj)
        {
            if (OperateurSelectionne == null) return;

            try
            {
                _repository.Delete(OperateurSelectionne);
                //Operateurs.Remove(OperateurSelectionne);
                Rafraichir();
                OperateurSelectionne = null;
                NomSelectionne = string.Empty;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
                OperateurSelectionne = null;
                NomSelectionne = string.Empty;
            }
        }
    }
}