using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class ModeleViewModel : BaseViewModel
    {
        private readonly ModeleRepository _repository;
        private readonly MarqueRepository _marqueRepository;
        private readonly MaterielRepository _materielRepository;

        public ObservableCollection<Modele> Modeles { get; set; }
        public ObservableCollection<Marque> Marques { get; set; }
        public ObservableCollection<Materiel> Materiels { get; set; }

        private string _nomSelectionne;
        public string NomSelectionne
        {
            get => _nomSelectionne;
            set { _nomSelectionne = value; OnPropertyChanged(); ValidateNom(); }
        }

        private string _cheminPhotoSelectionne;
        public string CheminPhotoSelectionne
        {
            get => _cheminPhotoSelectionne;
            set { _cheminPhotoSelectionne = value; OnPropertyChanged(); ValidateChemin(); }
        }

        private bool _actifSelectionne = true;
        public bool ActifSelectionne
        {
            get => _actifSelectionne;
            set { _actifSelectionne = value; OnPropertyChanged(); }
        }

        private Marque _marqueSelectionnee;
        public Marque MarqueSelectionnee
        {
            get => _marqueSelectionnee;
            set { _marqueSelectionnee = value; OnPropertyChanged(); }
        }

        private Materiel _materielSelectionne;
        public Materiel MaterielSelectionne
        {
            get => _materielSelectionne;
            set { _materielSelectionne = value; OnPropertyChanged(); }
        }

        private Modele _modeleSelectionne;
        public Modele ModeleSelectionne
        {
            get => _modeleSelectionne;
            set
            {
                _modeleSelectionne = value;
                OnPropertyChanged();
                if (value != null)
                {
                    NomSelectionne = value.Nom;
                    CheminPhotoSelectionne = value.CheminPhoto;
                    ActifSelectionne = value.Actif;
                    MarqueSelectionnee = Marques.FirstOrDefault(m => m.Id == value.MarqueId);
                    MaterielSelectionne = Materiels.FirstOrDefault(m => m.Id == value.MaterielId);
                }
            }
        }

        private string _filtreNom = string.Empty;
        public string FiltreNom
        {
            get => _filtreNom;
            set { _filtreNom = value; OnPropertyChanged(); AppliquerFiltre(); }
        }

        private Marque _filtreMarque;
        public Marque FiltreMarque
        {
            get => _filtreMarque;
            set { _filtreMarque = value; OnPropertyChanged(); AppliquerFiltre(); }
        }

        private Materiel _filtreMateriel;
        public Materiel FiltreMateriel
        {
            get => _filtreMateriel;
            set { _filtreMateriel = value; OnPropertyChanged(); AppliquerFiltre(); }
        }

        private string _erreurNom;
        public string ErreurNom
        {
            get => _erreurNom;
            set { _erreurNom = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        private string _erreurChemin;
        public string ErreurChemin
        {
            get => _erreurChemin;
            set { _erreurChemin = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        public bool HasErreur => !string.IsNullOrWhiteSpace(ErreurNom) ||
                                  !string.IsNullOrWhiteSpace(ErreurChemin);

        private string _erreurGlobale;
        public string ErreurGlobale
        {
            get => _erreurGlobale;
            set { _erreurGlobale = value; OnPropertyChanged(); }
        }

        public ICommand AjouterModeleCommand { get; }
        public ICommand ModifierModeleCommand { get; }
        public ICommand SupprimerModeleCommand { get; }
        public ICommand ParcourirPhotoCommand { get; }
        public ICommand ReinitialiserFiltreCommand { get; }
        public ICommand ActualiserCommand { get; }

        public ModeleViewModel(ModeleRepository repository,
                               MarqueRepository marqueRepository,
                               MaterielRepository materielRepository)
        {
            _repository = repository;
            _marqueRepository = marqueRepository;
            _materielRepository = materielRepository;

            Modeles = new ObservableCollection<Modele>();
            Marques = new ObservableCollection<Marque>();
            Materiels = new ObservableCollection<Materiel>();

            AjouterModeleCommand = new AsyncRelayCommand(AjouterModele);
            ModifierModeleCommand = new AsyncRelayCommand(ModifierModele);
            SupprimerModeleCommand = new AsyncRelayCommand(SupprimerModele);
            ParcourirPhotoCommand = new RelayCommand(ParcourirPhoto);
            ReinitialiserFiltreCommand = new RelayCommand(ReinitialiserFiltre);
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
            else if (Modeles.Any(m => m.Nom.ToLower() == NomSelectionne.ToLower() &&
                                      (ModeleSelectionne == null || m.Id != ModeleSelectionne.Id)))
                ErreurNom = "Nom déjà utilisé";
            else
                ErreurNom = string.Empty;
        }

        private void ValidateChemin()
        {
            if (string.IsNullOrWhiteSpace(CheminPhotoSelectionne))
                ErreurChemin = "Chemin photo obligatoire";
            else
                ErreurChemin = string.Empty;
        }

        private bool FiltrerModele(object obj)
        {
            if (obj is not Modele modele) return false;

            bool nomOk = string.IsNullOrWhiteSpace(FiltreNom) ||
                         modele.Nom.ToLower().Contains(FiltreNom.ToLower());

            bool marqueOk = FiltreMarque == null ||
                            modele.MarqueId == FiltreMarque.Id;

            bool materielOk = FiltreMateriel == null ||
                              modele.MaterielId == FiltreMateriel.Id;

            return nomOk && marqueOk && materielOk;
        }

        private void AppliquerFiltre()
        {
            System.Windows.Data.CollectionViewSource.GetDefaultView(Modeles).Refresh();
        }

        private void ReinitialiserFiltre(object obj)
        {
            FiltreNom = string.Empty;
            FiltreMarque = null;
            FiltreMateriel = null;
        }

        private void ParcourirPhoto(object obj)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                Title = "Sélectionner une photo"
            };

            if (dialog.ShowDialog() == true)
                CheminPhotoSelectionne = System.IO.Path.GetFileName(dialog.FileName);
        }

        public async Task Rafraichir()
        {
            var liste = await _repository.GetAll();
            var marques = await _marqueRepository.GetAll();
            var materiels = await _materielRepository.GetAll();

            App.Current.Dispatcher.Invoke(() =>
            {
                Modeles.Clear();
                foreach (var m in liste)
                    Modeles.Add(m);

                Marques.Clear();
                foreach (var m in marques)
                    Marques.Add(m);

                Materiels.Clear();
                foreach (var m in materiels)
                    Materiels.Add(m);

                var view = System.Windows.Data.CollectionViewSource.GetDefaultView(Modeles);
                view.Filter = FiltrerModele;

                // Sélectionner la première ligne
                ModeleSelectionne = Modeles.FirstOrDefault();

            });
        }

        public void EffacerChamps()
        {
            ModeleSelectionne = null;
            NomSelectionne = string.Empty;
            CheminPhotoSelectionne = string.Empty;
            ActifSelectionne = true;
            MarqueSelectionnee = null;
            MaterielSelectionne = null;
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
            ErreurNom = string.Empty;
            ErreurChemin = string.Empty;
        }

        private async Task AjouterModele(object obj)
        {
            ValidateNom();
            ValidateChemin();

            if (MarqueSelectionnee == null)
            {
                ErreurGlobale = "Veuillez sélectionner une marque.";
                return;
            }
            if (MaterielSelectionne == null)
            {
                ErreurGlobale = "Veuillez sélectionner un matériel.";
                return;
            }
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom + " " + ErreurChemin;
                return;
            }

            var modele = new Modele
            {
                Nom = NomSelectionne,
                CheminPhoto = CheminPhotoSelectionne,
                Actif = ActifSelectionne,
                MarqueId = MarqueSelectionnee.Id,
                MaterielId = MaterielSelectionne.Id
            };

            try
            {
                await _repository.Add(modele);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task ModifierModele(object obj)
        {
            if (ModeleSelectionne == null) return;
            ValidateNom();
            ValidateChemin();

            if (MarqueSelectionnee == null)
            {
                ErreurGlobale = "Veuillez sélectionner une marque.";
                return;
            }
            if (MaterielSelectionne == null)
            {
                ErreurGlobale = "Veuillez sélectionner un matériel.";
                return;
            }
            if (HasErreur)
            {
                ErreurGlobale = ErreurNom + " " + ErreurChemin;
                return;
            }

            var ancienNom = ModeleSelectionne.Nom;
            var ancienChemin = ModeleSelectionne.CheminPhoto;
            var ancienActif = ModeleSelectionne.Actif;
            var ancienMarqueId = ModeleSelectionne.MarqueId;
            var ancienMaterielId = ModeleSelectionne.MaterielId;

            ModeleSelectionne.Nom = NomSelectionne;
            ModeleSelectionne.CheminPhoto = CheminPhotoSelectionne;
            ModeleSelectionne.Actif = ActifSelectionne;
            ModeleSelectionne.MarqueId = MarqueSelectionnee.Id;
            ModeleSelectionne.MaterielId = MaterielSelectionne.Id;

            try
            {
                await _repository.Update(ModeleSelectionne);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                ModeleSelectionne.Nom = ancienNom;
                ModeleSelectionne.CheminPhoto = ancienChemin;
                ModeleSelectionne.Actif = ancienActif;
                ModeleSelectionne.MarqueId = ancienMarqueId;
                ModeleSelectionne.MaterielId = ancienMaterielId;
                ErreurGlobale = ex.Message;
            }
        }

        private async Task SupprimerModele(object obj)
        {
            if (ModeleSelectionne == null) return;

            try
            {
                await _repository.Delete(ModeleSelectionne);
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