using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class EdsLiaisonViewModel : BaseViewModel
    {
        private readonly EdsLiaisonRepository _repository;
        private readonly EdsRepository _edsRepository;

        public ObservableCollection<EdsLiaison> EdsLiaisons { get; set; }
        public ObservableCollection<Eds> EdsList { get; set; }

        private string _cibleSelectionne;
        public string CibleSelectionne
        {
            get => _cibleSelectionne;
            set { _cibleSelectionne = value; OnPropertyChanged(); ValidateCible(); }
        }

        private Eds _edsSelectionne;
        public Eds EdsSelectionne
        {
            get => _edsSelectionne;
            set { _edsSelectionne = value; OnPropertyChanged(); }
        }

        private EdsLiaison _edsLiaisonSelectionnee;
        public EdsLiaison EdsLiaisonSelectionnee
        {
            get => _edsLiaisonSelectionnee;
            set
            {
                _edsLiaisonSelectionnee = value;
                OnPropertyChanged();
                if (value != null)
                {
                    CibleSelectionne = value.Cible;
                    EdsSelectionne = EdsList.FirstOrDefault(e => e.Id == value.EdsId);
                }
            }
        }

        private string _erreurCible;
        public string ErreurCible
        {
            get => _erreurCible;
            set { _erreurCible = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        public bool HasErreur => !string.IsNullOrWhiteSpace(ErreurCible);

        private string _erreurGlobale;
        public string ErreurGlobale
        {
            get => _erreurGlobale;
            set { _erreurGlobale = value; OnPropertyChanged(); }
        }

        public ICommand AjouterCommand { get; }
        public ICommand ModifierCommand { get; }
        public ICommand SupprimerCommand { get; }
        public ICommand ActualiserCommand { get; }

        public EdsLiaisonViewModel(EdsLiaisonRepository repository, EdsRepository edsRepository)
        {
            _repository = repository;
            _edsRepository = edsRepository;
            EdsLiaisons = new ObservableCollection<EdsLiaison>();
            EdsList = new ObservableCollection<Eds>();

            AjouterCommand = new AsyncRelayCommand(Ajouter);
            ModifierCommand = new AsyncRelayCommand(Modifier);
            SupprimerCommand = new AsyncRelayCommand(Supprimer);
            ActualiserCommand = new AsyncRelayCommand(async _ =>
            {
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            });
        }

        private void ValidateCible()
        {
            if (string.IsNullOrWhiteSpace(CibleSelectionne))
                ErreurCible = "Cible obligatoire";
            else if (EdsLiaisons.Any(e => e.Cible.ToLower() == CibleSelectionne.ToLower() &&
                                          (EdsLiaisonSelectionnee == null || e.Id != EdsLiaisonSelectionnee.Id)))
                ErreurCible = "Cible déjà utilisée";
            else
                ErreurCible = string.Empty;
        }

        public async Task Rafraichir()
        {
            var liaisons = await _repository.GetAll();
            //var count = liaisons.Count; // point d'arrêt ici (F9 sur cette ligne)
            var eds = await _edsRepository.GetAll();
            App.Current.Dispatcher.Invoke(() =>
            {
                EdsLiaisons.Clear();
                foreach (var l in liaisons)
                    EdsLiaisons.Add(l);

                EdsList.Clear();
                foreach (var e in eds)
                    EdsList.Add(e);
            });
        }

        public void EffacerChamps()
        {
            EdsLiaisonSelectionnee = null;
            CibleSelectionne = string.Empty;
            EdsSelectionne = null;
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
            ErreurCible = string.Empty;
        }

        private async Task Ajouter(object obj)
        {
            ValidateCible();
            if (HasErreur)
            {
                ErreurGlobale = ErreurCible;
                return;
            }

            if (EdsSelectionne == null)
            {
                ErreurGlobale = "Veuillez sélectionner un EDS.";
                return;
            }

            var liaison = new EdsLiaison
            {
                Cible = CibleSelectionne,
                EdsId = EdsSelectionne.Id
            };

            try
            {
                await _repository.Add(liaison);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task Modifier(object obj)
        {
            if (EdsLiaisonSelectionnee == null) return;
            ValidateCible();
            if (HasErreur)
            {
                ErreurGlobale = ErreurCible;
                return;
            }

            if (EdsSelectionne == null)
            {
                ErreurGlobale = "Veuillez sélectionner un EDS.";
                return;
            }

            var ancienCible = EdsLiaisonSelectionnee.Cible;
            var ancienEdsId = EdsLiaisonSelectionnee.EdsId;

            EdsLiaisonSelectionnee.Cible = CibleSelectionne;
            EdsLiaisonSelectionnee.EdsId = EdsSelectionne.Id;

            try
            {
                await _repository.Update(EdsLiaisonSelectionnee);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                EdsLiaisonSelectionnee.Cible = ancienCible;
                EdsLiaisonSelectionnee.EdsId = ancienEdsId;
                ErreurGlobale = ex.Message;
            }
        }

        private async Task Supprimer(object obj)
        {
            if (EdsLiaisonSelectionnee == null) return;

            try
            {
                await _repository.Delete(EdsLiaisonSelectionnee);
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