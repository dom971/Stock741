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
                             FicheViewModel ficheViewModel)
        {
            _marqueViewModel = marqueViewModel;
            _materielViewModel = materielViewModel;
            _modeleViewModel = modeleViewModel;
            _requeteViewModel = requeteViewModel;
            _lieuViewModel = lieuViewModel;
            _ficheViewModel = ficheViewModel;

            NaviguerVersCommand = new RelayCommand(NaviguerVers);

            QuitterCommand = new RelayCommand(_ => System.Windows.Application.Current.Shutdown());

            //Vue par défaut
            VueActuelle = _marqueViewModel;
            TitreVueActuelle = "Marques";
            _lieuViewModel = lieuViewModel;
            _ficheViewModel = ficheViewModel;
        }

        private void NaviguerVers(object parametre)
        {
            switch (parametre)
            {
                case "Lieux":
                    VueActuelle = _lieuViewModel;
                    TitreVueActuelle = "Lieux";
                    VueActive = "Lieux";
                    break;
                case "Marques":
                    VueActuelle = _marqueViewModel;
                    TitreVueActuelle = "Marques";
                    VueActive = "Marques";
                    break;
                case "Materiels":
                    VueActuelle = _materielViewModel;
                    TitreVueActuelle = "Matériels";
                    VueActive = "Materiels";
                    break;
                case "Fiches":
                    VueActuelle = _ficheViewModel;
                    TitreVueActuelle = "Fiches";
                    VueActive = "Fiches";
                    break;
                case "Modeles":
                    _modeleViewModel.Rafraichir();
                    VueActuelle = _modeleViewModel;
                    TitreVueActuelle = "Modèles";
                    VueActive = "Modeles";
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
