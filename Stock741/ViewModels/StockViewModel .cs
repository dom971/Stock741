using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class StockViewModel : BaseViewModel
    {
        private readonly StockRepository _repository;
        private readonly ModeleRepository _modeleRepository;
        private readonly MarqueRepository _marqueRepository;
        private readonly MaterielRepository _materielRepository;
        private readonly StatutRepository _statutRepository;
        private readonly LieuRepository _lieuRepository;
        private readonly FournisseurRepository _fournisseurRepository;
        private readonly SystemeRepository _systemeRepository;

        private readonly string _effectuePar = Environment.UserName;
        private const string StatutCreationNom = "Stock";
        private const string NumReceptionDefaut = "0";
        private const string ColisDefaut = "0";

        public ObservableCollection<Stock> Stocks { get; set; }
        public ObservableCollection<Modele> Modeles { get; set; }
        public ObservableCollection<Modele> ModelesFiltres { get; set; }
        public ObservableCollection<Marque> Marques { get; set; }
        public ObservableCollection<Marque> MarquesFiltrees { get; set; }
        public ObservableCollection<Materiel> Materiels { get; set; }
        public ObservableCollection<Statut> Statuts { get; set; }
        public ObservableCollection<Lieu> Lieux { get; set; }
        public ObservableCollection<Fournisseur> Fournisseurs { get; set; }
        public ObservableCollection<Systeme> Systemes { get; set; }

        private Stock _detail;
        public Stock Detail
        {
            get => _detail;
            set { _detail = value; OnPropertyChanged(); }
        }

        private bool _peutModifier = true;
        public bool PeutModifier
        {
            get => _peutModifier;
            set
            {
                _peutModifier = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PeutChoisirStatut));
                OnPropertyChanged(nameof(PeutSupprimer));
                OnPropertyChanged(nameof(PeutModifierGarantie));
                OnPropertyChanged(nameof(AfficherModificationLimitee));
            }
        }

        public bool PeutChoisirStatut => StockSelectionne != null && PeutModifier;
        public bool PeutSupprimer => StockSelectionne != null && PeutModifier;
        public bool PeutModifierGarantie => PeutModifier && SousGarantieSelectionne;
        public bool AfficherModificationLimitee => StockSelectionne != null && !PeutModifier;

        // Photo
        private string _cheminPhoto;
        public string CheminPhoto
        {
            get => _cheminPhoto;
            set { _cheminPhoto = value; OnPropertyChanged(); }
        }

        // Champs formulaire
        private string _assetSelectionne;
        public string AssetSelectionne
        {
            get => _assetSelectionne;
            set { _assetSelectionne = value; OnPropertyChanged(); ValidateAsset(); }
        }

        private string _numSerieSelectionne;
        public string NumSerieSelectionne
        {
            get => _numSerieSelectionne;
            set { _numSerieSelectionne = value; OnPropertyChanged(); ValidateNumSerie(); }
        }

        private DateTime _dateSelectionne = DateTime.Now;
        public DateTime DateSelectionne
        {
            get => _dateSelectionne;
            set { _dateSelectionne = value; OnPropertyChanged(); }
        }

        private string _numReceptionSelectionne = NumReceptionDefaut;
        public string NumReceptionSelectionne
        {
            get => _numReceptionSelectionne;
            set { _numReceptionSelectionne = value; OnPropertyChanged(); ValidateNumReception(); }
        }

        private int _qteSelectionne = 1;
        public int QteSelectionne
        {
            get => _qteSelectionne;
            set { _qteSelectionne = value; OnPropertyChanged(); ValidateQte(); }
        }

        private DateTime? _garantieSelectionnee;
        public DateTime? GarantieSelectionnee
        {
            get => _garantieSelectionnee;
            set { _garantieSelectionnee = value; OnPropertyChanged(); }
        }

        private bool _sousGarantieSelectionne = true;
        public bool SousGarantieSelectionne
        {
            get => _sousGarantieSelectionne;
            set
            {
                _sousGarantieSelectionne = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PeutModifierGarantie));

                if (!value)
                    GarantieSelectionnee = null;
            }
        }

        private string _colisSelectionne = ColisDefaut;
        public string ColisSelectionne
        {
            get => _colisSelectionne;
            set { _colisSelectionne = value; OnPropertyChanged(); ValidateColis(); }
        }

        // Filtrage en cascade
        private Materiel _materielSelectionne;
        public Materiel MaterielSelectionne
        {
            get => _materielSelectionne;
            set
            {
                _materielSelectionne = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AfficherTelephonie));
                OnPropertyChanged(nameof(AfficherSysteme));
                FiltrerMarques();
                MarqueSelectionnee = null;
                ModeleSelectionne = null;
                CheminPhoto = null;

                if (!AfficherTelephonie)
                {
                    NumSimSelectionne = string.Empty;
                    Imei1Selectionne = string.Empty;
                    Imei2Selectionne = string.Empty;
                }

                if (!AfficherSysteme)
                    SystemeSelectionne = null;
            }
        }

        public bool AfficherTelephonie =>
            EstMaterielTelephonie(MaterielSelectionne?.Nom);

        public bool AfficherSysteme =>
            EstMaterielOrdinateur(MaterielSelectionne?.Nom);

        private Marque _marqueSelectionnee;
        public Marque MarqueSelectionnee
        {
            get => _marqueSelectionnee;
            set
            {
                _marqueSelectionnee = value;
                OnPropertyChanged();
                FiltrerModeles();
                ModeleSelectionne = null;
                CheminPhoto = null;
            }
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
                    if (MarqueSelectionnee?.Id != value.MarqueId)
                    {
                        _marqueSelectionnee = MarquesFiltrees.FirstOrDefault(m => m.Id == value.MarqueId) ??
                                             Marques.FirstOrDefault(m => m.Id == value.MarqueId);
                        OnPropertyChanged(nameof(MarqueSelectionnee));
                    }

                    CheminPhoto = value.CheminPhoto;
                }
                else
                {
                    CheminPhoto = null;
                }
            }
        }

        private Fournisseur _fournisseurSelectionne;
        public Fournisseur FournisseurSelectionne
        {
            get => _fournisseurSelectionne;
            set { _fournisseurSelectionne = value; OnPropertyChanged(); }
        }

        private Statut _statutSelectionne;
        public Statut StatutSelectionne
        {
            get => _statutSelectionne;
            set { _statutSelectionne = value; OnPropertyChanged(); }
        }

        private Lieu _lieuSelectionne;
        public Lieu LieuSelectionne
        {
            get => _lieuSelectionne;
            set { _lieuSelectionne = value; OnPropertyChanged(); }
        }

        private Systeme _systemeSelectionne;
        public Systeme SystemeSelectionne
        {
            get => _systemeSelectionne;
            set { _systemeSelectionne = value; OnPropertyChanged(); }
        }

        private string _numSimSelectionne;
        public string NumSimSelectionne
        {
            get => _numSimSelectionne;
            set { _numSimSelectionne = value; OnPropertyChanged(); }
        }

        private string _imei1Selectionne;
        public string Imei1Selectionne
        {
            get => _imei1Selectionne;
            set { _imei1Selectionne = value; OnPropertyChanged(); }
        }

        private string _imei2Selectionne;
        public string Imei2Selectionne
        {
            get => _imei2Selectionne;
            set { _imei2Selectionne = value; OnPropertyChanged(); }
        }

        private Stock _stockSelectionne;
        public Stock StockSelectionne
        {
            get => _stockSelectionne;
            set
            {
                _stockSelectionne = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PeutChoisirStatut));
                OnPropertyChanged(nameof(PeutSupprimer));
                if (value != null)
                    _ = ChargerDetailAsync(value.Id);
                else
                {
                    Detail = null;
                    PeutModifier = true;
                    CheminPhoto = null;
                }
            }
        }

        // Filtre recherche
        private string _filtreNom = string.Empty;
        public string FiltreNom
        {
            get => _filtreNom;
            set { _filtreNom = value; OnPropertyChanged(); AppliquerFiltre(); }
        }

        // Erreurs
        private string _erreurAsset;
        public string ErreurAsset
        {
            get => _erreurAsset;
            set { _erreurAsset = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        private string _erreurNumSerie;
        public string ErreurNumSerie
        {
            get => _erreurNumSerie;
            set { _erreurNumSerie = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        private string _erreurNumReception;
        public string ErreurNumReception
        {
            get => _erreurNumReception;
            set { _erreurNumReception = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        private string _erreurColis;
        public string ErreurColis
        {
            get => _erreurColis;
            set { _erreurColis = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        private string _erreurQte;
        public string ErreurQte
        {
            get => _erreurQte;
            set { _erreurQte = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        private string _erreurLieu;
        public string ErreurLieu
        {
            get => _erreurLieu;
            set { _erreurLieu = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        private string _erreurFournisseur;
        public string ErreurFournisseur
        {
            get => _erreurFournisseur;
            set { _erreurFournisseur = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        private string _erreurGlobale;
        public string ErreurGlobale
        {
            get => _erreurGlobale;
            set { _erreurGlobale = value; OnPropertyChanged(); }
        }

        public bool HasErreur => !string.IsNullOrWhiteSpace(ErreurAsset) ||
                                  !string.IsNullOrWhiteSpace(ErreurNumSerie) ||
                                  !string.IsNullOrWhiteSpace(ErreurNumReception) ||
                                  !string.IsNullOrWhiteSpace(ErreurColis) ||
                                  !string.IsNullOrWhiteSpace(ErreurQte) ||
                                  !string.IsNullOrWhiteSpace(ErreurLieu) ||
                                  !string.IsNullOrWhiteSpace(ErreurFournisseur);

        public ICommand AjouterCommand { get; }
        public ICommand ModifierCommand { get; }
        public ICommand SupprimerCommand { get; }
        public ICommand ActualiserCommand { get; }
        public ICommand NouveauCommand { get; }

        public StockViewModel(StockRepository repository,
                              ModeleRepository modeleRepository,
                              MarqueRepository marqueRepository,
                              MaterielRepository materielRepository,
                              StatutRepository statutRepository,
                              LieuRepository lieuRepository,
                              FournisseurRepository fournisseurRepository,
                              SystemeRepository systemeRepository)
        {
            _repository = repository;
            _modeleRepository = modeleRepository;
            _marqueRepository = marqueRepository;
            _materielRepository = materielRepository;
            _statutRepository = statutRepository;
            _lieuRepository = lieuRepository;
            _fournisseurRepository = fournisseurRepository;
            _systemeRepository = systemeRepository;

            Stocks = new ObservableCollection<Stock>();
            Modeles = new ObservableCollection<Modele>();
            ModelesFiltres = new ObservableCollection<Modele>();
            Marques = new ObservableCollection<Marque>();
            MarquesFiltrees = new ObservableCollection<Marque>();
            Materiels = new ObservableCollection<Materiel>();
            Statuts = new ObservableCollection<Statut>();
            Lieux = new ObservableCollection<Lieu>();
            Fournisseurs = new ObservableCollection<Fournisseur>();
            Systemes = new ObservableCollection<Systeme>();

            AjouterCommand = new AsyncRelayCommand(Ajouter);
            ModifierCommand = new AsyncRelayCommand(Modifier);
            SupprimerCommand = new AsyncRelayCommand(Supprimer);
            NouveauCommand = new RelayCommand(_ =>
            {
                EffacerChamps();
                EffacerErreur();
            });
            ActualiserCommand = new AsyncRelayCommand(async _ =>
            {
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            });
        }

        private void FiltrerMarques()
        {
            MarquesFiltrees.Clear();
            if (MaterielSelectionne == null)
            {
                foreach (var m in Marques)
                    MarquesFiltrees.Add(m);
                return;
            }

            var marquesFiltrees = Modeles
                .Where(m => m.MaterielId == MaterielSelectionne.Id)
                .Select(m => m.Marque)
                .Where(m => m != null)
                .DistinctBy(m => m.Id)
                .OrderBy(m => m.Nom)
                .ToList();

            foreach (var m in marquesFiltrees)
                MarquesFiltrees.Add(m);
        }

        private void FiltrerModeles()
        {
            ModelesFiltres.Clear();
            if (MarqueSelectionnee == null && MaterielSelectionne == null)
            {
                foreach (var m in Modeles)
                    ModelesFiltres.Add(m);
                return;
            }

            var modelesFiltres = Modeles
                .Where(m =>
                    (MaterielSelectionne == null || m.MaterielId == MaterielSelectionne.Id) &&
                    (MarqueSelectionnee == null || m.MarqueId == MarqueSelectionnee.Id))
                .OrderBy(m => m.Nom)
                .ToList();

            foreach (var m in modelesFiltres)
                ModelesFiltres.Add(m);
        }

        private static bool EstMaterielTelephonie(string nomMateriel)
        {
            if (string.IsNullOrWhiteSpace(nomMateriel))
                return false;

            return nomMateriel.Contains("smartphone", StringComparison.OrdinalIgnoreCase) ||
                   nomMateriel.Contains("dongle", StringComparison.OrdinalIgnoreCase) ||
                   nomMateriel.Contains("téléphone satellite", StringComparison.OrdinalIgnoreCase) ||
                   nomMateriel.Contains("telephone satellite", StringComparison.OrdinalIgnoreCase);
        }

        private static bool EstMaterielOrdinateur(string nomMateriel)
        {
            if (string.IsNullOrWhiteSpace(nomMateriel))
                return false;

            return nomMateriel.Contains("ordinateur", StringComparison.OrdinalIgnoreCase);
        }

        private Statut GetStatutCreation()
        {
            return Statuts.FirstOrDefault(s =>
                string.Equals(s.Nom, StatutCreationNom, StringComparison.OrdinalIgnoreCase));
        }

        private void ValidateAsset()
        {
            if (string.IsNullOrWhiteSpace(AssetSelectionne))
            {
                ErreurAsset = string.Empty;
                return;
            }
            if (!Regex.IsMatch(AssetSelectionne, @"^GU\d{6}$"))
                ErreurAsset = "Format invalide — ex: GU000777";
            else if (Stocks.Any(s => s.Asset?.ToLower() == AssetSelectionne.ToLower() &&
                                     (StockSelectionne == null || s.Id != StockSelectionne.Id)))
                ErreurAsset = "Asset déjà utilisé";
            else
                ErreurAsset = string.Empty;
        }

        private void ValidateNumSerie()
        {
            if (string.IsNullOrWhiteSpace(NumSerieSelectionne))
                ErreurNumSerie = "Numéro de série obligatoire";
            else
                ErreurNumSerie = string.Empty;
        }

        private void ValidateNumReception()
        {
            ErreurNumReception = EstEntierNaturelOuVide(NumReceptionSelectionne)
                ? string.Empty
                : "N° réception doit être un entier naturel";
        }

        private void ValidateColis()
        {
            ErreurColis = EstEntierNaturelOuVide(ColisSelectionne)
                ? string.Empty
                : "Colis doit être un entier naturel";
        }

        private void ValidateQte()
        {
            ErreurQte = QteSelectionne > 0
                ? string.Empty
                : "Quantité doit être supérieure à 0";
        }

        private void ValidateLieu()
        {
            ErreurLieu = LieuSelectionne == null
                ? "Veuillez sélectionner un lieu"
                : string.Empty;
        }

        private void ValidateFournisseur()
        {
            ErreurFournisseur = FournisseurSelectionne == null
                ? "Veuillez sélectionner un fournisseur"
                : string.Empty;
        }

        private static bool EstEntierNaturelOuVide(string valeur)
        {
            return string.IsNullOrWhiteSpace(valeur) ||
                   int.TryParse(valeur, out var nombre) && nombre >= 0;
        }

        private static string NormaliserEntierNaturel(string valeur)
        {
            return string.IsNullOrWhiteSpace(valeur) ? "0" : valeur.Trim();
        }

        private void AppliquerFiltre()
        {
            var view = System.Windows.Data.CollectionViewSource.GetDefaultView(Stocks);
            if (view != null)
                view.Filter = o => o is Stock s &&
                    (string.IsNullOrWhiteSpace(FiltreNom) ||
                     (s.Asset?.ToLower().Contains(FiltreNom.ToLower()) ?? false) ||
                     (s.NumSerie?.ToLower().Contains(FiltreNom.ToLower()) ?? false) ||
                     (s.Modele?.Nom?.ToLower().Contains(FiltreNom.ToLower()) ?? false));
        }

        private async Task ChargerDetailAsync(int id)
        {
            var stock = await _repository.GetById(id);
            Detail = stock;

            if (stock != null)
            {
                PeutModifier = !await _repository.HasAffectation(id);

                // Filtrage en cascade
                MaterielSelectionne = Materiels.FirstOrDefault(m =>
                    m.Id == stock.Modele?.MaterielId);
                MarqueSelectionnee = MarquesFiltrees.FirstOrDefault(m =>
                    m.Id == stock.Modele?.MarqueId) ?? Marques.FirstOrDefault(m =>
                    m.Id == stock.Modele?.MarqueId);
                ModeleSelectionne = Modeles.FirstOrDefault(m =>
                    m.Id == stock.ModeleId);

                AssetSelectionne = stock.Asset;
                NumSerieSelectionne = stock.NumSerie;
                DateSelectionne = stock.Date;
                NumReceptionSelectionne = stock.NumReception;
                QteSelectionne = stock.Qte;
                SousGarantieSelectionne = stock.SousGarantie;
                GarantieSelectionnee = stock.Garantie;
                ColisSelectionne = stock.Colis;
                FournisseurSelectionne = Fournisseurs.FirstOrDefault(f => f.Id == stock.FournisseurId);
                StatutSelectionne = Statuts.FirstOrDefault(s => s.Id == stock.StatutId);
                LieuSelectionne = Lieux.FirstOrDefault(l => l.Id == stock.LieuId);
                SystemeSelectionne = AfficherSysteme
                    ? Systemes.FirstOrDefault(s => s.Id == stock.SystemeId)
                    : null;
                NumSimSelectionne = stock.NumSim;
                Imei1Selectionne = stock.Imei1;
                Imei2Selectionne = stock.Imei2;
                CheminPhoto = stock.Modele?.CheminPhoto;
            }
        }

        public async Task Rafraichir()
        {
            var stocks = await _repository.GetAll();
            var modeles = await _modeleRepository.GetAll();
            var marques = await _marqueRepository.GetAll();
            var materiels = await _materielRepository.GetAll();
            var statuts = await _statutRepository.GetAll();
            var lieux = await _lieuRepository.GetAll();
            var fournisseurs = await _fournisseurRepository.GetAll();
            var systemes = await _systemeRepository.GetAll();

            App.Current.Dispatcher.Invoke(() =>
            {
                Stocks.Clear();
                foreach (var s in stocks) Stocks.Add(s);

                Modeles.Clear();
                foreach (var m in modeles) Modeles.Add(m);

                Marques.Clear();
                foreach (var m in marques) Marques.Add(m);

                Materiels.Clear();
                foreach (var m in materiels) Materiels.Add(m);

                Statuts.Clear();
                foreach (var s in statuts) Statuts.Add(s);

                Lieux.Clear();
                foreach (var l in lieux) Lieux.Add(l);

                Fournisseurs.Clear();
                foreach (var f in fournisseurs) Fournisseurs.Add(f);

                Systemes.Clear();
                foreach (var s in systemes) Systemes.Add(s);

                FiltrerMarques();
                FiltrerModeles();
                if (StockSelectionne == null)
                    StatutSelectionne = GetStatutCreation();
                AppliquerFiltre();
            });
        }

        public void EffacerChamps()
        {
            _stockSelectionne = null;
            OnPropertyChanged(nameof(StockSelectionne));
            OnPropertyChanged(nameof(PeutChoisirStatut));
            OnPropertyChanged(nameof(PeutSupprimer));
            Detail = null;
            PeutModifier = true;
            CheminPhoto = null;
            MaterielSelectionne = null;
            MarqueSelectionnee = null;
            ModeleSelectionne = null;
            AssetSelectionne = string.Empty;
            NumSerieSelectionne = string.Empty;
            DateSelectionne = DateTime.Now;
            NumReceptionSelectionne = NumReceptionDefaut;
            QteSelectionne = 1;
            SousGarantieSelectionne = true;
            GarantieSelectionnee = null;
            ColisSelectionne = ColisDefaut;
            FournisseurSelectionne = null;
            StatutSelectionne = GetStatutCreation();
            LieuSelectionne = null;
            SystemeSelectionne = null;
            NumSimSelectionne = string.Empty;
            Imei1Selectionne = string.Empty;
            Imei2Selectionne = string.Empty;
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
            ErreurAsset = string.Empty;
            ErreurNumSerie = string.Empty;
            ErreurNumReception = string.Empty;
            ErreurColis = string.Empty;
            ErreurQte = string.Empty;
            ErreurLieu = string.Empty;
            ErreurFournisseur = string.Empty;
        }

        private async Task Ajouter(object obj)
        {
            ValidateNumSerie();
            ValidateAsset();
            ValidateNumReception();
            ValidateColis();
            ValidateQte();
            ValidateLieu();
            ValidateFournisseur();
            if (HasErreur)
            {
                ErreurGlobale = string.Join(" ", new[] { ErreurAsset, ErreurNumSerie, ErreurNumReception, ErreurColis, ErreurQte, ErreurLieu, ErreurFournisseur }
                    .Where(e => !string.IsNullOrWhiteSpace(e)));
                return;
            }

            if (ModeleSelectionne == null)
            {
                ErreurGlobale = "Veuillez sélectionner un modèle.";
                return;
            }

            var statutCreation = GetStatutCreation();
            if (statutCreation == null)
            {
                ErreurGlobale = "Le statut Stock est introuvable.";
                return;
            }

            if (AfficherSysteme && SystemeSelectionne == null)
            {
                ErreurGlobale = "Veuillez sélectionner un système.";
                return;
            }

            var stock = new Stock
            {
                Asset = AssetSelectionne,
                NumSerie = NumSerieSelectionne,
                Date = DateSelectionne,
                NumReception = NormaliserEntierNaturel(NumReceptionSelectionne),
                Qte = QteSelectionne,
                SousGarantie = SousGarantieSelectionne,
                Garantie = SousGarantieSelectionne ? GarantieSelectionnee : null,
                Colis = NormaliserEntierNaturel(ColisSelectionne),
                ModeleId = ModeleSelectionne.Id,
                FournisseurId = FournisseurSelectionne?.Id,
                StatutId = statutCreation.Id,
                LieuId = LieuSelectionne?.Id,
                SystemeId = AfficherSysteme ? SystemeSelectionne?.Id : null,
                NumSim = NumSimSelectionne,
                Imei1 = Imei1Selectionne,
                Imei2 = Imei2Selectionne
            };

            try
            {
                await _repository.Add(stock, _effectuePar);
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
            if (StockSelectionne == null) return;
            if (!PeutModifier)
            {
                ErreurGlobale = "Impossible de modifier : ce matériel a des affectations.";
                return;
            }

            ValidateNumSerie();
            ValidateAsset();
            ValidateNumReception();
            ValidateColis();
            ValidateQte();
            ValidateLieu();
            ValidateFournisseur();
            if (HasErreur)
            {
                ErreurGlobale = string.Join(" ", new[] { ErreurAsset, ErreurNumSerie, ErreurNumReception, ErreurColis, ErreurQte, ErreurLieu, ErreurFournisseur }
                    .Where(e => !string.IsNullOrWhiteSpace(e)));
                return;
            }

            if (ModeleSelectionne == null)
            {
                ErreurGlobale = "Veuillez sélectionner un modèle.";
                return;
            }

            if (AfficherSysteme && SystemeSelectionne == null)
            {
                ErreurGlobale = "Veuillez sélectionner un système.";
                return;
            }

            var stock = new Stock
            {
                Id = StockSelectionne.Id,
                RowVersion = Detail?.RowVersion ?? StockSelectionne.RowVersion,
                Asset = AssetSelectionne,
                NumSerie = NumSerieSelectionne,
                Date = DateSelectionne,
                NumReception = NormaliserEntierNaturel(NumReceptionSelectionne),
                Qte = QteSelectionne,
                SousGarantie = SousGarantieSelectionne,
                Garantie = SousGarantieSelectionne ? GarantieSelectionnee : null,
                Colis = NormaliserEntierNaturel(ColisSelectionne),
                ModeleId = ModeleSelectionne.Id,
                FournisseurId = FournisseurSelectionne?.Id,
                StatutId = StatutSelectionne?.Id,
                LieuId = LieuSelectionne?.Id,
                SystemeId = AfficherSysteme ? SystemeSelectionne?.Id : null,
                NumSim = NumSimSelectionne,
                Imei1 = Imei1Selectionne,
                Imei2 = Imei2Selectionne
            };

            try
            {
                await _repository.Update(stock, _effectuePar);
                await Rafraichir();
                EffacerChamps();
                EffacerErreur();
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task Supprimer(object obj)
        {
            if (StockSelectionne == null) return;

            try
            {
                await _repository.Delete(StockSelectionne);
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
