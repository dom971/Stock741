
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class ForfaitViewModel : BaseViewModel
    {
        private readonly ForfaitRepository _repository;
        private readonly OperateurRepository _operateurRepository;

        public ObservableCollection<Forfait> Forfaits { get; set; }
        public ObservableCollection<Operateur> Operateurs { get; set; }

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

        private Operateur _operateurSelectionne;
        public Operateur OperateurSelectionne
        {
            get => _operateurSelectionne;
            set { _operateurSelectionne = value; OnPropertyChanged(); }
        }

        private Forfait _forfaitSelectionne;
        public Forfait ForfaitSelectionne
        {
            get => _forfaitSelectionne;
            set
            {
                _forfaitSelectionne = value;
                OnPropertyChanged();
                if (value != null)
                {
                    NomSelectionne = value.Nom;
                    ActifSelectionne = value.Actif;
                    OperateurSelectionne = Operateurs.FirstOrDefault(o => o.Id == value.OperateurId);
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

        public ICommand AjouterForfaitCommand { get; }
        public ICommand ModifierForfaitCommand { get; }
        public ICommand SupprimerForfaitCommand { get; }

        public ForfaitViewModel(ForfaitRepository repository, OperateurRepository operateurRepository)
        {
            _repository = repository;
            _operateurRepository = operateurRepository;
            Forfaits = new ObservableCollection<Forfait>(_repository.GetAll());
            Operateurs = new ObservableCollection<Operateur>(_operateurRepository.GetAll());

            AjouterForfaitCommand = new RelayCommand(AjouterForfait);
            ModifierForfaitCommand = new RelayCommand(ModifierForfait);
            SupprimerForfaitCommand = new RelayCommand(SupprimerForfait);
        }

        public void Rafraichir()
        {
            Forfaits.Clear();
            foreach (var m in _repository.GetAll())
                Forfaits.Add(m);
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
        }

        private void ValidateNom()
        {
            if (string.IsNullOrWhiteSpace(NomSelectionne))
                ErreurNom = "Nom obligatoire";
            else if (Forfaits.Any(f => f.Nom.ToLower() == NomSelectionne.ToLower() &&
                                       (ForfaitSelectionne == null || f.Id != ForfaitSelectionne.Id)))
                ErreurNom = "Nom déjà utilisé";
            else
                ErreurNom = string.Empty;
        }

        private void AjouterForfait(object obj)
        {
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            if (OperateurSelectionne == null)
            {
                ErreurGlobale = "Veuillez sélectionner un opérateur.";
                return;
            }

            var forfait = new Forfait
            {
                Nom = NomSelectionne,
                Actif = ActifSelectionne,
                OperateurId = OperateurSelectionne.Id
            };

            try
            {
                _repository.Add(forfait);
                forfait.Operateur = OperateurSelectionne;
                //Forfaits.Add(forfait);
                Rafraichir();
                NomSelectionne = string.Empty;
                ActifSelectionne = true;
                OperateurSelectionne = null;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private void ModifierForfait(object obj)
        {
            if (ForfaitSelectionne == null) return;
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom;
                return;
            }

            if (OperateurSelectionne == null)
            {
                ErreurGlobale = "Veuillez sélectionner un opérateur.";
                return;
            }

            var ancienNom = ForfaitSelectionne.Nom;
            var ancienActif = ForfaitSelectionne.Actif;
            var ancienOperateurId = ForfaitSelectionne.OperateurId;
            var ancienOperateur = ForfaitSelectionne.Operateur;

            ForfaitSelectionne.Nom = NomSelectionne;
            ForfaitSelectionne.Actif = ActifSelectionne;
            ForfaitSelectionne.OperateurId = OperateurSelectionne.Id;
            ForfaitSelectionne.Operateur = OperateurSelectionne;

            try
            {
                _repository.Update(ForfaitSelectionne);
                //CollectionViewSource.GetDefaultView(Forfaits).Refresh();
                Rafraichir();
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ForfaitSelectionne.Nom = ancienNom;
                ForfaitSelectionne.Actif = ancienActif;
                ForfaitSelectionne.OperateurId = ancienOperateurId;
                ForfaitSelectionne.Operateur = ancienOperateur;
                ErreurGlobale = ex.Message;
            }
        }

        private void SupprimerForfait(object obj)
        {
            if (ForfaitSelectionne == null) return;

            try
            {
                _repository.Delete(ForfaitSelectionne);
                //Forfaits.Remove(ForfaitSelectionne);
                Rafraichir();
                ForfaitSelectionne = null;
                NomSelectionne = string.Empty;
                ActifSelectionne = true;
                OperateurSelectionne = null;
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
                ForfaitSelectionne = null;
                NomSelectionne = string.Empty;
                ActifSelectionne = true;
                OperateurSelectionne = null;
            }
        }
    }
}