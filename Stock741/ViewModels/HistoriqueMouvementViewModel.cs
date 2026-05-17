using System.Collections.ObjectModel;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class HistoriqueMouvementViewModel : BaseViewModel
    {
        private readonly HistoriqueMouvementRepository _repository;

        public ObservableCollection<HistoriqueMouvement> Historiques { get; set; }

        private HistoriqueMouvement _historiqueSelectionne;
        public HistoriqueMouvement HistoriqueSelectionne
        {
            get => _historiqueSelectionne;
            set
            {
                _historiqueSelectionne = value;
                OnPropertyChanged();
                Detail = value;
            }
        }

        private HistoriqueMouvement _detail;
        public HistoriqueMouvement Detail
        {
            get => _detail;
            set { _detail = value; OnPropertyChanged(); }
        }

        private string _filtre = string.Empty;
        public string Filtre
        {
            get => _filtre;
            set { _filtre = value; OnPropertyChanged(); AppliquerFiltre(); }
        }

        private string _erreurGlobale;
        public string ErreurGlobale
        {
            get => _erreurGlobale;
            set { _erreurGlobale = value; OnPropertyChanged(); }
        }

        public ICommand ActualiserCommand { get; }

        public HistoriqueMouvementViewModel(HistoriqueMouvementRepository repository)
        {
            _repository = repository;
            Historiques = new ObservableCollection<HistoriqueMouvement>();

            ActualiserCommand = new AsyncRelayCommand(async _ =>
            {
                await Rafraichir();
                EffacerErreur();
            });
        }

        public async Task Rafraichir()
        {
            try
            {
                var liste = await _repository.GetAll();
                App.Current.Dispatcher.Invoke(() =>
                {
                    Historiques.Clear();
                    foreach (var h in liste)
                        Historiques.Add(h);

                    AppliquerFiltre();
                });

                Detail = null;
                _historiqueSelectionne = null;
                OnPropertyChanged(nameof(HistoriqueSelectionne));
            }
            catch (Exception ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
        }

        private void AppliquerFiltre()
        {
            var view = System.Windows.Data.CollectionViewSource.GetDefaultView(Historiques);
            if (view == null) return;

            view.Filter = o =>
            {
                if (o is not HistoriqueMouvement h)
                    return false;

                if (string.IsNullOrWhiteSpace(Filtre))
                    return true;

                var filtre = Filtre.Trim();
                return Contient(h.TypeMouvement, filtre) ||
                       Contient(h.Stock?.Asset, filtre) ||
                       Contient(h.Stock?.NumSerie, filtre) ||
                       Contient(h.Stock?.Modele?.Nom, filtre) ||
                       Contient(h.Stock?.Modele?.Marque?.Nom, filtre) ||
                       Contient(h.Stock?.Modele?.Materiel?.Nom, filtre) ||
                       Contient(h.AncienStatut?.Nom, filtre) ||
                       Contient(h.AncienLieu?.Nom, filtre) ||
                       Contient(h.NouveauStatut?.Nom, filtre) ||
                       Contient(h.NouveauLieu?.Nom, filtre) ||
                       Contient(h.AncienUtilisateur?.Nom, filtre) ||
                       Contient(h.AncienUtilisateur?.Prenom, filtre) ||
                       Contient(h.AncienEds?.Nom, filtre) ||
                       Contient(h.EffectuePar, filtre) ||
                       Contient(h.Commentaire, filtre);
            };
        }

        private static bool Contient(string valeur, string filtre)
        {
            return valeur?.Contains(filtre, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}
