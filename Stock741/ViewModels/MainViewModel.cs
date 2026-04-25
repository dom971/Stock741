
using System.Windows.Input;
using Stock741.Commands;
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

        private object _vueActuelle;
        public object VueActuelle
        {
            get => _vueActuelle;
            set { _vueActuelle = value; OnPropertyChanged(); }
        }

        private string _titreVueActuelle;
        public string TitreVueActuelle
        {
            get => _titreVueActuelle;
            set { _titreVueActuelle = value; OnPropertyChanged(); }
        }

        private string _vueActive = "Marques";
        public string VueActive
        {
            get => _vueActive;
            set { _vueActive = value; OnPropertyChanged(); }
        }

        public ICommand NaviguerVersCommand { get; }
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
                             EdsViewModel edsViewModel)
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

            NaviguerVersCommand = new AsyncRelayCommand(NaviguerVers);
            QuitterCommand = new RelayCommand(_ => System.Windows.Application.Current.Shutdown());

            // Vue par défaut
            VueActuelle = _marqueViewModel;
            TitreVueActuelle = "Marques";
            VueActive = "Marques";
            _ = _marqueViewModel.Rafraichir();


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
    }
}