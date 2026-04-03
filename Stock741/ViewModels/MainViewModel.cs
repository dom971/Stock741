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

        public ICommand NaviguerVersCommand { get; }

        public MainViewModel(MarqueViewModel marqueViewModel,
                             MaterielViewModel materielViewModel,
                             ModeleViewModel modeleViewModel,
                             RequeteViewModel requeteViewModel)
        {
            _marqueViewModel = marqueViewModel;
            _materielViewModel = materielViewModel;
            _modeleViewModel = modeleViewModel;
            _requeteViewModel = requeteViewModel;

            NaviguerVersCommand = new RelayCommand(NaviguerVers);

            // Vue par défaut
            VueActuelle = _marqueViewModel;

            TitreVueActuelle = "Marques";
        }

        private void NaviguerVers(object parametre)
        {
            switch (parametre)
            {
                case "Marques":
                    VueActuelle = _marqueViewModel;
                    TitreVueActuelle = "Marques";
                    break;
                case "Materiels":
                    VueActuelle = _materielViewModel;
                    TitreVueActuelle = "Matériels";
                    break;
                case "Modeles":
                    _modeleViewModel.Rafraichir();
                    VueActuelle = _modeleViewModel;
                    TitreVueActuelle = "Modèles";
                    break;
                case "Requetes":
                    VueActuelle = _requeteViewModel;
                    TitreVueActuelle = "Requêtes";
                    break;
                default:
                    VueActuelle = _marqueViewModel;
                    TitreVueActuelle = "Marques";
                    break;
            }
        }
    }
}
