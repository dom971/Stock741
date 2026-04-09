
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

            NaviguerVersCommand = new RelayCommand(NaviguerVers);
            QuitterCommand = new RelayCommand(_ => System.Windows.Application.Current.Shutdown());

            // Vue par défaut
            VueActuelle = _marqueViewModel;
            TitreVueActuelle = "Marques";
            VueActive = "Marques";
            _ = _marqueViewModel.Rafraichir();
        }

        private void NaviguerVers(object parametre)
        {
            switch (parametre)
            {
                case "Marques":
                    _ = _marqueViewModel.Rafraichir();
                    _marqueViewModel.EffacerErreur();
                    VueActuelle = _marqueViewModel;
                    TitreVueActuelle = "Marques";
                    VueActive = "Marques";
                    break;
                case "Materiels":
                    _ = _materielViewModel.Rafraichir();
                    _materielViewModel.EffacerErreur();
                    VueActuelle = _materielViewModel;
                    TitreVueActuelle = "Matériels";
                    VueActive = "Materiels";
                    break;
                case "Modeles":
                    _ = _modeleViewModel.Rafraichir();
                    _modeleViewModel.EffacerErreur();
                    VueActuelle = _modeleViewModel;
                    TitreVueActuelle = "Modèles";
                    VueActive = "Modeles";
                    break;
                case "Lieux":
                    _ = _lieuViewModel.Rafraichir();
                    _lieuViewModel.EffacerErreur();
                    VueActuelle = _lieuViewModel;
                    TitreVueActuelle = "Lieux";
                    VueActive = "Lieux";
                    break;
                case "Fiches":
                    _ = _ficheViewModel.Rafraichir();
                    _ficheViewModel.EffacerErreur();
                    VueActuelle = _ficheViewModel;
                    TitreVueActuelle = "Fiches";
                    VueActive = "Fiches";
                    break;
                case "Statuts":
                    _ = _statutViewModel.Rafraichir();
                    _statutViewModel.EffacerErreur();
                    VueActuelle = _statutViewModel;
                    TitreVueActuelle = "Statuts";
                    VueActive = "Statuts";
                    break;
                case "Fournisseurs":
                    _ = _fournisseurViewModel.Rafraichir();
                    _fournisseurViewModel.EffacerErreur();
                    VueActuelle = _fournisseurViewModel;
                    TitreVueActuelle = "Fournisseurs";
                    VueActive = "Fournisseurs";
                    break;
                case "Operateurs":
                    _ = _operateurViewModel.Rafraichir();
                    _operateurViewModel.EffacerErreur();
                    VueActuelle = _operateurViewModel;
                    TitreVueActuelle = "Opérateurs";
                    VueActive = "Operateurs";
                    break;
                case "Forfaits":
                    _ = _forfaitViewModel.Rafraichir();
                    _forfaitViewModel.EffacerErreur();
                    VueActuelle = _forfaitViewModel;
                    TitreVueActuelle = "Forfaits";
                    VueActive = "Forfaits";
                    break;
                case "Eds":
                    _ = _edsViewModel.Rafraichir();
                    _edsViewModel.EffacerErreur();
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

