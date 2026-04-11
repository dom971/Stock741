using System.Collections.ObjectModel;
using System.Threading.Tasks;
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
        public ICommand ActualiserCommand { get; }

        public ForfaitViewModel(ForfaitRepository repository, OperateurRepository operateurRepository)
        {
            _repository = repository;
            _operateurRepository = operateurRepository;
            Forfaits = new ObservableCollection<Forfait>();
            Operateurs = new ObservableCollection<Operateur>();

            AjouterForfaitCommand = new AsyncRelayCommand(AjouterForfait);
            ModifierForfaitCommand = new AsyncRelayCommand(ModifierForfait);
            SupprimerForfaitCommand = new AsyncRelayCommand(SupprimerForfait);
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
            else if (Forfaits.Any(f => f.Nom.ToLower() == NomSelectionne.ToLower() &&
                                       (ForfaitSelectionne == null || f.Id != ForfaitSelectionne.Id)))
                ErreurNom = "Nom déjà utilisé";
            else
                ErreurNom = string.Empty;
        }

        public async Task Rafraichir()
        {
            var forfaits = await _repository.GetAll();
            var operateurs = await _operateurRepository.GetAll();
            App.Current.Dispatcher.Invoke(() =>
            {
                Forfaits.Clear();
                foreach (var f in forfaits)
                    Forfaits.Add(f);

                Operateurs.Clear();
                foreach (var o in operateurs)
                    Operateurs.Add(o);
            });
        }

        public void EffacerChamps()
        {
            ForfaitSelectionne = null;
            NomSelectionne = string.Empty;
            ActifSelectionne = true;
            OperateurSelectionne = null;
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
            ErreurNom = string.Empty;
        }

        private async Task AjouterForfait(object obj)
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
                await _repository.Add(forfait);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task ModifierForfait(object obj)
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
                await _repository.Update(ForfaitSelectionne);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
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

        private async Task SupprimerForfait(object obj)
        {
            if (ForfaitSelectionne == null) return;

            try
            {
                await _repository.Delete(ForfaitSelectionne);
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