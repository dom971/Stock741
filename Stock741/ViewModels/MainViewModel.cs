
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.ViewModels;

namespace Stock741.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly MarqueViewModel _marqueViewModel;
        private readonly MaterielViewModel _materielViewModel;
        private readonly ModeleViewModel _modeleViewModel;
        private readonly RequeteViewModel _requeteViewModel;
        private readonly LieuViewModel _lieuViewModel;
        private readonly FicheViewModel _ficheViewModel;
        private readonly StatutViewModel _statutViewModel;
        private readonly FournisseurViewModel _fournisseurViewModel;
        private readonly OperateurViewModel _operateurViewModel;
        private readonly ForfaitViewModel _forfaitViewModel;
        private readonly EdsViewModel _edsViewModel;
        private readonly EdsLiaisonViewModel _edsLiaisonViewModel;
        private readonly UtilisateurViewModel _utilisateurViewModel;
        private readonly SystemeViewModel _systemeViewModel;
        private readonly StockViewModel _stockViewModel;
        private readonly AffectationViewModel _affectationViewModel;
        private readonly HistoriqueMouvementViewModel _historiqueMouvementViewModel;

        private object _vueActuelle;
        public object VueActuelle
        {
            get => _vueActuelle;
            set
            {
                if (_vueActuelle is BaseViewModel ancienViewModel)
                    ancienViewModel.PropertyChanged -= VueActuelle_PropertyChanged;

                _vueActuelle = value;

                if (_vueActuelle is BaseViewModel nouveauViewModel)
                    nouveauViewModel.PropertyChanged += VueActuelle_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCurrentViewBusy));
            }
        }

        private string _titreVueActuelle;
        public string TitreVueActuelle
        {
            get => _titreVueActuelle;
            set { _titreVueActuelle = value; OnPropertyChanged(); }
        }

        private string _vueActive = "Stock";
        public string VueActive
        {
            get => _vueActive;
            set { _vueActive = value; OnPropertyChanged(); }
        }

        public bool IsCurrentViewBusy => VueActuelle is BaseViewModel viewModel && viewModel.IsBusy;

        private void VueActuelle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BaseViewModel.IsBusy))
                OnPropertyChanged(nameof(IsCurrentViewBusy));
        }

        public ICommand NaviguerVersCommand { get; }
        public ICommand AffecterStockSelectionneCommand { get; }
        public ICommand VoirStockDepuisAffectationCommand { get; }
        public ICommand QuitterCommand { get; }

        public MainViewModel(MarqueViewModel marqueViewModel,
                             MaterielViewModel materielViewModel,
                             ModeleViewModel modeleViewModel,
                             RequeteViewModel requeteViewModel,
                             LieuViewModel lieuViewModel,
                             FicheViewModel ficheViewModel,
                             StatutViewModel statutViewModel,
                             FournisseurViewModel fournisseurViewModel,
                             OperateurViewModel operateurViewModel,
                             ForfaitViewModel forfaitViewModel,
                             EdsViewModel edsViewModel,
                             EdsLiaisonViewModel edsLiaisonViewModel, UtilisateurViewModel utilisateurViewModel, SystemeViewModel systemeViewModel, StockViewModel stockViewModel, AffectationViewModel affectationViewModel, HistoriqueMouvementViewModel historiqueMouvementViewModel)
        {
            _marqueViewModel = marqueViewModel;
            _materielViewModel = materielViewModel;
            _modeleViewModel = modeleViewModel;
            _requeteViewModel = requeteViewModel;
            _lieuViewModel = lieuViewModel;
            _ficheViewModel = ficheViewModel;
            _statutViewModel = statutViewModel;
            _fournisseurViewModel = fournisseurViewModel;
            _operateurViewModel = operateurViewModel;
            _forfaitViewModel = forfaitViewModel;
            _edsViewModel = edsViewModel;
            _edsLiaisonViewModel = edsLiaisonViewModel;
            _utilisateurViewModel = utilisateurViewModel;
            _systemeViewModel = systemeViewModel;
            _stockViewModel = stockViewModel;
            _affectationViewModel = affectationViewModel;
            _historiqueMouvementViewModel = historiqueMouvementViewModel;

            NaviguerVersCommand = new AsyncRelayCommand(NaviguerVers);
            AffecterStockSelectionneCommand = new AsyncRelayCommand(AffecterStockSelectionne);
            VoirStockDepuisAffectationCommand = new AsyncRelayCommand(VoirStockDepuisAffectation);
            QuitterCommand = new RelayCommand(_ => System.Windows.Application.Current.Shutdown());

            // Vue par défaut
            VueActuelle = _stockViewModel;
            TitreVueActuelle = "Stock";
            VueActive = "Stock";
            _ = _stockViewModel.Rafraichir();


            // Warm-up SQL Server — pré-charge Eds en arrière-plan
            _ = Task.Run(async () =>
            {
                try
                {
                    await _edsViewModel.Rafraichir();
                }
                catch { }
            });


        }

        private async Task NaviguerVers(object parametre)
        {
            switch (parametre)
            {
                case "Stock":
                    _stockViewModel.EffacerChamps();
                    _stockViewModel.EffacerErreur();
                    await _stockViewModel.Rafraichir();
                    VueActuelle = _stockViewModel;
                    TitreVueActuelle = "Stock";
                    VueActive = "Stock";
                    break;

                case "HistoriqueMouvements":
                    _historiqueMouvementViewModel.EffacerErreur();
                    await _historiqueMouvementViewModel.Rafraichir();
                    VueActuelle = _historiqueMouvementViewModel;
                    TitreVueActuelle = "Historique mouvements";
                    VueActive = "HistoriqueMouvements";
                    break;

                case "Affectations":
                    _affectationViewModel.EffacerChamps();
                    await _affectationViewModel.Rafraichir();
                    VueActuelle = _affectationViewModel;
                    TitreVueActuelle = "Affectations";
                    VueActive = "Affectations";
                    break;

                case "Marques":
                    _marqueViewModel.EffacerChamps();
                    _marqueViewModel.EffacerErreur();
                    await _marqueViewModel.Rafraichir();
                    VueActuelle = _marqueViewModel;
                    TitreVueActuelle = "Marques";
                    VueActive = "Marques";
                    break;
                case "Materiels":
                    _materielViewModel.EffacerChamps();
                    _materielViewModel.EffacerErreur();
                    await _materielViewModel.Rafraichir();
                    VueActuelle = _materielViewModel;
                    TitreVueActuelle = "Matériels";
                    VueActive = "Materiels";
                    break;
                case "Modeles":
                    _modeleViewModel.EffacerChamps();
                    _modeleViewModel.EffacerErreur();
                    await _modeleViewModel.Rafraichir();
                    VueActuelle = _modeleViewModel;
                    TitreVueActuelle = "Modèles";
                    VueActive = "Modeles";
                    break;
                case "Lieux":
                    _lieuViewModel.EffacerChamps();
                    _lieuViewModel.EffacerErreur();
                    await _lieuViewModel.Rafraichir();
                    VueActuelle = _lieuViewModel;
                    TitreVueActuelle = "Lieux";
                    VueActive = "Lieux";
                    break;
                case "Fiches":
                    _ficheViewModel.EffacerChamps();
                    _ficheViewModel.EffacerErreur();
                    await _ficheViewModel.Rafraichir();
                    VueActuelle = _ficheViewModel;
                    TitreVueActuelle = "Fiches";
                    VueActive = "Fiches";
                    break;
                case "Statuts":
                    _statutViewModel.EffacerChamps();
                    _statutViewModel.EffacerErreur();
                    await _statutViewModel.Rafraichir();
                    VueActuelle = _statutViewModel;
                    TitreVueActuelle = "Statuts";
                    VueActive = "Statuts";
                    break;
                case "Systemes":
                    _systemeViewModel.EffacerChamps();
                    _systemeViewModel.EffacerErreur();
                    await _systemeViewModel.Rafraichir();
                    VueActuelle = _systemeViewModel;
                    TitreVueActuelle = "Systèmes";
                    VueActive = "Systemes";
                    break;
                case "Fournisseurs":
                    _fournisseurViewModel.EffacerChamps();
                    _fournisseurViewModel.EffacerErreur();
                    await _fournisseurViewModel.Rafraichir();
                    VueActuelle = _fournisseurViewModel;
                    TitreVueActuelle = "Fournisseurs";
                    VueActive = "Fournisseurs";
                    break;
                case "Operateurs":
                    _operateurViewModel.EffacerChamps();
                    _operateurViewModel.EffacerErreur();
                    await _operateurViewModel.Rafraichir();
                    VueActuelle = _operateurViewModel;
                    TitreVueActuelle = "Opérateurs";
                    VueActive = "Operateurs";
                    break;
                case "Forfaits":
                    _forfaitViewModel.EffacerChamps();
                    _forfaitViewModel.EffacerErreur();
                    await _forfaitViewModel.Rafraichir();
                    VueActuelle = _forfaitViewModel;
                    TitreVueActuelle = "Forfaits";
                    VueActive = "Forfaits";
                    break;
                case "Eds":
                    _edsViewModel.EffacerChamps();
                    _edsViewModel.EffacerErreur();
                    await _edsViewModel.Rafraichir();
                    VueActuelle = _edsViewModel;
                    TitreVueActuelle = "EDS";
                    VueActive = "Eds";
                    break;
                case "EdsLiaisons":
                    _edsLiaisonViewModel.EffacerChamps();
                    _edsLiaisonViewModel.EffacerErreur();
                    await _edsLiaisonViewModel.Rafraichir();
                    VueActuelle = _edsLiaisonViewModel;
                    TitreVueActuelle = "EDS Liaisons";
                    VueActive = "EdsLiaisons";
                    break;
                case "Utilisateurs":
                    _utilisateurViewModel.EffacerErreur();
                    await _utilisateurViewModel.Rafraichir();
                    VueActuelle = _utilisateurViewModel;
                    TitreVueActuelle = "Utilisateurs";
                    VueActive = "Utilisateurs";
                    break;
                case "Requetes":
                    VueActuelle = _requeteViewModel;
                    TitreVueActuelle = "Requêtes";
                    VueActive = "Requetes";
                    break;
                default:
                    VueActuelle = _marqueViewModel;
                    TitreVueActuelle = "Marques";
                    break;
            }
        }

        private async Task AffecterStockSelectionne(object parametre)
        {
            if (parametre is not Stock stock)
                return;

            _affectationViewModel.EffacerChamps();
            await _affectationViewModel.Rafraichir();
            await _affectationViewModel.SelectionnerDepuisStockAsync(stock.Id);

            VueActuelle = _affectationViewModel;
            TitreVueActuelle = "Affectations";
            VueActive = "Affectations";
        }

        private async Task VoirStockDepuisAffectation(object parametre)
        {
            var stockId = _affectationViewModel.GetStockCourantId();
            if (stockId == null)
                return;

            _stockViewModel.EffacerChamps();
            _stockViewModel.EffacerErreur();
            await _stockViewModel.Rafraichir();
            await _stockViewModel.SelectionnerStockAsync(stockId.Value);

            VueActuelle = _stockViewModel;
            TitreVueActuelle = "Stock";
            VueActive = "Stock";
        }
    }
}
