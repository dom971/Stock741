using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class AffectationViewModel : BaseViewModel
    {
        private readonly AffectationRepository _affectationRepository;
        private readonly UtilisateurRepository _utilisateurRepository;
        private readonly EdsRepository _edsRepository;
        private readonly EdsLiaisonRepository _edsLiaisonRepository;
        private readonly OperateurRepository _operateurRepository;
        private readonly ForfaitRepository _forfaitRepository;
        private readonly StatutRepository _statutRepository;
        private readonly List<Statut> _tousStatutsAffectation = new();
        private bool _resetEnCours;

        public ObservableCollection<Affectation> Affectations { get; } = new();
        public ObservableCollection<Stock> StocksDisponibles { get; } = new();
        public ObservableCollection<Utilisateur> Utilisateurs { get; } = new();
        public ObservableCollection<Eds> EdsListe { get; } = new();
        public ObservableCollection<Operateur> Operateurs { get; } = new();
        public ObservableCollection<Forfait> Forfaits { get; } = new();
        public ObservableCollection<Statut> StatutsAffectation { get; } = new();
        public ObservableCollection<Statut> StatutsRetour { get; } = new();

        private Affectation? _affectationSelectionnee;
        public Affectation? AffectationSelectionnee
        {
            get => _affectationSelectionnee;
            set
            {
                _affectationSelectionnee = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PeutRetourner));
                OnPropertyChanged(nameof(PeutModifier));
                OnPropertyChanged(nameof(AfficherModifier));
                OnPropertyChanged(nameof(EstModification));
                OnPropertyChanged(nameof(PeutAjouter));
                OnPropertyChanged(nameof(PeutChoisirMateriel));
                OnPropertyChanged(nameof(PeutVoirStock));
                ActualiserStatutsAffectationDisponibles();
                CommandManager.InvalidateRequerySuggested();
                if (value != null)
                    _ = ChargerDetailAsync(value.Id);
            }
        }

        private Affectation? _detail;
        public Affectation? Detail
        {
            get => _detail;
            set { _detail = value; OnPropertyChanged(); }
        }

        private Stock? _stockSelectionne;
        public Stock? StockSelectionne
        {
            get => _stockSelectionne;
            set
            {
                _stockSelectionne = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AfficherPoste));
                OnPropertyChanged(nameof(AfficherTelephonie));
                OnPropertyChanged(nameof(AfficherReseau));
                OnPropertyChanged(nameof(PeutVoirStock));
                SelectionnerStatutParDefaut();
            }
        }

        private Statut? _statutAffectationSelectionne;
        public Statut? StatutAffectationSelectionne
        {
            get => _statutAffectationSelectionne;
            set
            {
                _statutAffectationSelectionne = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DateFinObligatoire));
            }
        }

        private Utilisateur? _utilisateurSelectionne;
        public Utilisateur? UtilisateurSelectionne
        {
            get => _utilisateurSelectionne;
            set
            {
                _utilisateurSelectionne = value;
                OnPropertyChanged();
                if (!_resetEnCours)
                    _ = SelectionnerEdsDepuisUtilisateurAsync();
            }
        }

        private Eds? _edsSelectionne;
        public Eds? EdsSelectionne
        {
            get => _edsSelectionne;
            set
            {
                _edsSelectionne = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EdsEstModifieManuellement));
                OnPropertyChanged(nameof(MessageEdsAutomatique));
            }
        }

        private Eds? _edsAutomatiqueSelectionne;
        public Eds? EdsAutomatiqueSelectionne
        {
            get => _edsAutomatiqueSelectionne;
            set
            {
                _edsAutomatiqueSelectionne = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EdsEstModifieManuellement));
                OnPropertyChanged(nameof(MessageEdsAutomatique));
            }
        }

        private Operateur? _operateurSelectionne;
        public Operateur? OperateurSelectionne
        {
            get => _operateurSelectionne;
            set { _operateurSelectionne = value; OnPropertyChanged(); FiltrerForfaits(); }
        }

        private Forfait? _forfaitSelectionne;
        public Forfait? ForfaitSelectionne
        {
            get => _forfaitSelectionne;
            set { _forfaitSelectionne = value; OnPropertyChanged(); }
        }

        private DateTime _dateDebut = DateTime.Today;
        public DateTime DateDebut
        {
            get => _dateDebut;
            set { _dateDebut = value; OnPropertyChanged(); }
        }

        private DateTime? _dateFin;
        public DateTime? DateFin
        {
            get => _dateFin;
            set { _dateFin = value; OnPropertyChanged(); }
        }

        private DateTime? _dateMouvement = DateTime.Today;
        public DateTime? DateMouvement
        {
            get => _dateMouvement;
            set { _dateMouvement = value; OnPropertyChanged(); }
        }

        private Statut? _statutRetourSelectionne;
        public Statut? StatutRetourSelectionne
        {
            get => _statutRetourSelectionne;
            set { _statutRetourSelectionne = value; OnPropertyChanged(); }
        }

        private string _nomAppareil = string.Empty;
        public string NomAppareil
        {
            get => _nomAppareil;
            set { _nomAppareil = value; OnPropertyChanged(); }
        }

        private string _adresseIP = string.Empty;
        public string AdresseIP
        {
            get => _adresseIP;
            set { _adresseIP = value; OnPropertyChanged(); }
        }

        private string _masqueIP = string.Empty;
        public string MasqueIP
        {
            get => _masqueIP;
            set { _masqueIP = value; OnPropertyChanged(); }
        }

        private string _passerelleIP = string.Empty;
        public string PasserelleIP
        {
            get => _passerelleIP;
            set { _passerelleIP = value; OnPropertyChanged(); }
        }

        private string _nomPC = string.Empty;
        public string NomPC
        {
            get => _nomPC;
            set { _nomPC = value; OnPropertyChanged(); }
        }

        private string _edsPC = string.Empty;
        public string EdsPC
        {
            get => _edsPC;
            set { _edsPC = value; OnPropertyChanged(); }
        }

        private string _ancienPC = string.Empty;
        public string AncienPC
        {
            get => _ancienPC;
            set { _ancienPC = value; OnPropertyChanged(); }
        }

        private string _numTelMobile = string.Empty;
        public string NumTelMobile
        {
            get => _numTelMobile;
            set { _numTelMobile = value; OnPropertyChanged(); }
        }

        private string _motif = string.Empty;
        public string Motif
        {
            get => _motif;
            set { _motif = value; OnPropertyChanged(); }
        }

        private string _commentaire = string.Empty;
        public string Commentaire
        {
            get => _commentaire;
            set { _commentaire = value; OnPropertyChanged(); }
        }

        private string _motifRetour = string.Empty;
        public string MotifRetour
        {
            get => _motifRetour;
            set { _motifRetour = value; OnPropertyChanged(); }
        }

        private string _commentaireRetour = string.Empty;
        public string CommentaireRetour
        {
            get => _commentaireRetour;
            set { _commentaireRetour = value; OnPropertyChanged(); }
        }

        private string _filtre = string.Empty;
        public string Filtre
        {
            get => _filtre;
            set { _filtre = value; OnPropertyChanged(); AppliquerFiltreAffectations(); }
        }

        private bool _afficherHistorique;
        public bool AfficherHistorique
        {
            get => _afficherHistorique;
            set { _afficherHistorique = value; OnPropertyChanged(); AppliquerFiltreAffectations(); }
        }

        private string _filtreMateriel = string.Empty;
        public string FiltreMateriel
        {
            get => _filtreMateriel;
            set { _filtreMateriel = value; OnPropertyChanged(); AppliquerFiltreStocksDisponibles(); }
        }

        private string _filtreUtilisateur = string.Empty;
        public string FiltreUtilisateur
        {
            get => _filtreUtilisateur;
            set
            {
                _filtreUtilisateur = value;
                OnPropertyChanged();
                _ = RechercherUtilisateursAsync();
            }
        }

        private string _filtreEds = string.Empty;
        public string FiltreEds
        {
            get => _filtreEds;
            set
            {
                _filtreEds = value;
                OnPropertyChanged();
                _ = RechercherEdsAsync();
            }
        }

        private string _erreurGlobale = string.Empty;
        public string ErreurGlobale
        {
            get => _erreurGlobale;
            set { _erreurGlobale = value; OnPropertyChanged(); }
        }

        private string _messageSucces = string.Empty;
        public string MessageSucces
        {
            get => _messageSucces;
            set { _messageSucces = value; OnPropertyChanged(); }
        }

        public bool PeutRetourner => AffectationSelectionnee?.Actif == true;
        public bool AfficherPoste => StockSelectionne?.Modele?.Materiel?.Nom?
            .Contains("ordinateur", StringComparison.OrdinalIgnoreCase) == true;
        public bool AfficherTelephonie
        {
            get
            {
                var materiel = StockSelectionne?.Modele?.Materiel?.Nom;
                if (string.IsNullOrWhiteSpace(materiel))
                    return false;

                return materiel.Contains("smartphone", StringComparison.OrdinalIgnoreCase)
                    || materiel.Contains("dongle", StringComparison.OrdinalIgnoreCase)
                    || materiel.Contains("4g", StringComparison.OrdinalIgnoreCase)
                    || materiel.Contains("téléphone satellite", StringComparison.OrdinalIgnoreCase)
                    || materiel.Contains("telephone satellite", StringComparison.OrdinalIgnoreCase);
            }
        }
        public bool AfficherReseau
        {
            get
            {
                var materiel = StockSelectionne?.Modele?.Materiel?.Nom;
                if (string.IsNullOrWhiteSpace(materiel))
                    return false;

                return materiel.Contains("copieur", StringComparison.OrdinalIgnoreCase)
                    || materiel.Contains("imprimante", StringComparison.OrdinalIgnoreCase)
                    || materiel.Contains("switch", StringComparison.OrdinalIgnoreCase);
            }
        }
        public bool EdsEstModifieManuellement =>
            EdsAutomatiqueSelectionne != null &&
            EdsSelectionne != null &&
            EdsAutomatiqueSelectionne.Id != EdsSelectionne.Id;

        public string MessageEdsAutomatique
        {
            get
            {
                if (EdsAutomatiqueSelectionne == null)
                    return string.Empty;

                var message = $"EDS automatique : {EdsAutomatiqueSelectionne.Cnx} - {EdsAutomatiqueSelectionne.Nom}";
                if (EdsEstModifieManuellement)
                    message += " (EDS affectation modifié manuellement)";

                return message;
            }
        }
        public bool EstModification => AffectationSelectionnee != null;
        public bool PeutAjouter => !EstModification;
        public bool PeutModifier => AffectationSelectionnee != null;
        public bool AfficherModifier => AffectationSelectionnee?.Actif == true;
        public bool PeutChoisirMateriel => !EstModification;
        public bool PeutVoirStock => AffectationSelectionnee?.StockId != null || StockSelectionne?.Id != null;
        public bool DateFinObligatoire => EstStatutSelectionne("pret");

        public ICommand AjouterCommand { get; }
        public ICommand ModifierCommand { get; }
        public ICommand RetournerCommand { get; }
        public ICommand NouveauCommand { get; }
        public ICommand ActualiserCommand { get; }

        public AffectationViewModel(
            AffectationRepository affectationRepository,
            UtilisateurRepository utilisateurRepository,
            EdsRepository edsRepository,
            EdsLiaisonRepository edsLiaisonRepository,
            OperateurRepository operateurRepository,
            ForfaitRepository forfaitRepository,
            StatutRepository statutRepository)
        {
            _affectationRepository = affectationRepository;
            _utilisateurRepository = utilisateurRepository;
            _edsRepository = edsRepository;
            _edsLiaisonRepository = edsLiaisonRepository;
            _operateurRepository = operateurRepository;
            _forfaitRepository = forfaitRepository;
            _statutRepository = statutRepository;

            AjouterCommand = new AsyncRelayCommand(Ajouter);
            ModifierCommand = new AsyncRelayCommand(Modifier, _ => PeutModifier);
            RetournerCommand = new AsyncRelayCommand(Retourner, _ => PeutRetourner);
            NouveauCommand = new RelayCommand(_ => EffacerChamps());
            ActualiserCommand = new AsyncRelayCommand(async _ =>
            {
                await RunBusyAsync(async () =>
                {
                    await Rafraichir();
                    EffacerErreur();
                });
            });
        }

        public async Task Rafraichir()
        {
            var affectations = await _affectationRepository.GetAll();
            var stocks = await _affectationRepository.GetStocksDisponibles();
            var operateurs = await _operateurRepository.GetAll();
            var forfaits = await _forfaitRepository.GetAll();
            var statuts = await _statutRepository.GetAll();
            var statutsAffectation = statuts
                .Where(s => string.Equals(s.Type, "Affectation", StringComparison.OrdinalIgnoreCase))
                .ToList();
            var statutsRetour = statuts
                .Where(s => string.Equals(s.Type, "Retour", StringComparison.OrdinalIgnoreCase))
                .ToList();

            App.Current.Dispatcher.Invoke(() =>
            {
                _tousStatutsAffectation.Clear();
                _tousStatutsAffectation.AddRange(statutsAffectation);
                Remplacer(Affectations, affectations);
                Remplacer(StocksDisponibles, stocks);
                Utilisateurs.Clear();
                EdsListe.Clear();
                Remplacer(Operateurs, operateurs);
                Remplacer(Forfaits, forfaits);
                ActualiserStatutsAffectationDisponibles();
                Remplacer(StatutsRetour, statutsRetour);
                StatutRetourSelectionne = GetStatutRetourDefaut();
                AppliquerFiltreAffectations();
                AppliquerFiltreStocksDisponibles();
                FiltrerForfaits();
            });

            Detail = null;
            AffectationSelectionnee = null;
            OnPropertyChanged(nameof(EstModification));
            OnPropertyChanged(nameof(PeutChoisirMateriel));
        }

        public void EffacerChamps()
        {
            _resetEnCours = true;
            _affectationSelectionnee = null;
            OnPropertyChanged(nameof(AffectationSelectionnee));
            ActualiserStatutsAffectationDisponibles();
            Detail = null;
            StockSelectionne = null;
            UtilisateurSelectionne = null;
            EdsSelectionne = null;
            EdsAutomatiqueSelectionne = null;
            OperateurSelectionne = null;
            ForfaitSelectionne = null;
            StatutAffectationSelectionne = null;
            FiltreMateriel = string.Empty;
            FiltreUtilisateur = string.Empty;
            FiltreEds = string.Empty;
            DateDebut = DateTime.Today;
            DateFin = null;
            DateMouvement = DateTime.Today;
            StatutRetourSelectionne = GetStatutRetourDefaut();
            NomAppareil = string.Empty;
            AdresseIP = string.Empty;
            MasqueIP = string.Empty;
            PasserelleIP = string.Empty;
            NomPC = string.Empty;
            EdsPC = string.Empty;
            AncienPC = string.Empty;
            NumTelMobile = string.Empty;
            Motif = string.Empty;
            Commentaire = string.Empty;
            MotifRetour = string.Empty;
            CommentaireRetour = string.Empty;
            Utilisateurs.Clear();
            EdsListe.Clear();
            OnPropertyChanged(nameof(EdsEstModifieManuellement));
            OnPropertyChanged(nameof(MessageEdsAutomatique));
            _resetEnCours = false;
            OnPropertyChanged(nameof(EstModification));
            OnPropertyChanged(nameof(PeutAjouter));
            OnPropertyChanged(nameof(PeutModifier));
            OnPropertyChanged(nameof(AfficherModifier));
            OnPropertyChanged(nameof(PeutRetourner));
            OnPropertyChanged(nameof(PeutChoisirMateriel));
            OnPropertyChanged(nameof(PeutVoirStock));
            CommandManager.InvalidateRequerySuggested();
            EffacerErreur();
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
            MessageSucces = string.Empty;
        }

        public Task SelectionnerDepuisStockAsync(int stockId)
        {
            var affectationActive = Affectations.FirstOrDefault(a => a.StockId == stockId && a.Actif);
            if (affectationActive != null)
            {
                AffectationSelectionnee = affectationActive;
                return Task.CompletedTask;
            }

            var stock = StocksDisponibles.FirstOrDefault(s => s.Id == stockId);
            if (stock == null)
            {
                ErreurGlobale = "Le matériel sélectionné n'est pas disponible pour une nouvelle affectation.";
                return Task.CompletedTask;
            }

            AffectationSelectionnee = null;
            StockSelectionne = stock;
            return Task.CompletedTask;
        }

        public int? GetStockCourantId()
        {
            return AffectationSelectionnee?.StockId ?? StockSelectionne?.Id;
        }

        private async Task ChargerDetailAsync(int id)
        {
            var detail = await _affectationRepository.GetById(id);
            Detail = detail;

            if (detail != null)
                ChargerFormulaireDepuisAffectation(detail);
        }

        private async Task Ajouter(object? parameter)
        {
            try
            {
                EffacerErreur();

                if (StockSelectionne == null)
                {
                    ErreurGlobale = "Le matériel est obligatoire.";
                    return;
                }

                if (UtilisateurSelectionne == null)
                {
                    ErreurGlobale = "L'utilisateur est obligatoire.";
                    return;
                }

                if (!ValiderStatutAffectation())
                    return;

                var affectation = new Affectation
                {
                    StockId = StockSelectionne.Id,
                    UtilisateurId = UtilisateurSelectionne.Id,
                    EdsId = EdsSelectionne?.Id,
                    EdsAutomatiqueId = EdsAutomatiqueSelectionne?.Id,
                    OperateurId = OperateurSelectionne?.Id,
                    ForfaitId = ForfaitSelectionne?.Id,
                    DateDebut = DateDebut.Date,
                    DateFin = DateFinObligatoire ? DateFin : null,
                    NomAppareil = NomAppareil?.Trim() ?? string.Empty,
                    AdresseIP = AdresseIP?.Trim() ?? string.Empty,
                    MasqueIP = MasqueIP?.Trim() ?? string.Empty,
                    PasserelleIP = PasserelleIP?.Trim() ?? string.Empty,
                    NomPC = NomPC?.Trim() ?? string.Empty,
                    EdsPC = EdsPC?.Trim() ?? string.Empty,
                    AncienPC = AncienPC?.Trim() ?? string.Empty,
                    NumTelMobile = NumTelMobile?.Trim() ?? string.Empty,
                    Motif = Motif?.Trim() ?? string.Empty,
                    Commentaire = Commentaire?.Trim() ?? string.Empty,
                    Actif = true
                };

                await RunBusyAsync(async () =>
                {
                    await _affectationRepository.Ajouter(affectation, StatutAffectationSelectionne!.Id, Environment.UserName);
                    await Rafraichir();
                    EffacerChamps();
                });
                MessageSucces = "Affectation enregistrée.";
            }
            catch (Exception ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task Modifier(object? parameter)
        {
            try
            {
                EffacerErreur();

                if (AffectationSelectionnee == null)
                    return;

                if (UtilisateurSelectionne == null)
                {
                    ErreurGlobale = "L'utilisateur est obligatoire.";
                    return;
                }

                if (!ValiderStatutAffectation())
                    return;

                var affectation = new Affectation
                {
                    Id = AffectationSelectionnee.Id,
                    StockId = AffectationSelectionnee.StockId,
                    RowVersion = AffectationSelectionnee.RowVersion,
                    UtilisateurId = UtilisateurSelectionne.Id,
                    EdsId = EdsSelectionne?.Id,
                    EdsAutomatiqueId = EdsAutomatiqueSelectionne?.Id,
                    OperateurId = OperateurSelectionne?.Id,
                    ForfaitId = ForfaitSelectionne?.Id,
                    DateDebut = DateDebut.Date,
                    DateFin = DateFinObligatoire ? DateFin : null,
                    NomAppareil = NomAppareil?.Trim() ?? string.Empty,
                    AdresseIP = AdresseIP?.Trim() ?? string.Empty,
                    MasqueIP = MasqueIP?.Trim() ?? string.Empty,
                    PasserelleIP = PasserelleIP?.Trim() ?? string.Empty,
                    NomPC = NomPC?.Trim() ?? string.Empty,
                    EdsPC = EdsPC?.Trim() ?? string.Empty,
                    AncienPC = AncienPC?.Trim() ?? string.Empty,
                    NumTelMobile = NumTelMobile?.Trim() ?? string.Empty,
                    Motif = Motif?.Trim() ?? string.Empty,
                    Commentaire = Commentaire?.Trim() ?? string.Empty
                };

                await RunBusyAsync(async () =>
                {
                    await _affectationRepository.Modifier(affectation, StatutAffectationSelectionne!.Id, Environment.UserName);
                    await Rafraichir();
                    EffacerChamps();
                });
                MessageSucces = "Affectation modifiée.";
            }
            catch (Exception ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private async Task Retourner(object? parameter)
        {
            try
            {
                EffacerErreur();

                if (AffectationSelectionnee == null)
                    return;

                if (!ValiderRetour())
                    return;

                await RunBusyAsync(async () =>
                {
                    await _affectationRepository.Retourner(
                        AffectationSelectionnee.Id,
                        DateTime.Now,
                        StatutRetourSelectionne!.Id,
                        MotifRetour?.Trim() ?? string.Empty,
                        CommentaireRetour?.Trim() ?? string.Empty,
                        Environment.UserName);
                    await Rafraichir();
                });
                MessageSucces = "Retour enregistré.";
            }
            catch (Exception ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private void AppliquerFiltreAffectations()
        {
            var view = CollectionViewSource.GetDefaultView(Affectations);
            if (view == null)
                return;

            view.Filter = o =>
            {
                if (o is not Affectation a)
                    return false;

                if (!AfficherHistorique && !a.Actif)
                    return false;

                if (string.IsNullOrWhiteSpace(Filtre))
                    return true;

                var filtre = Filtre.Trim().ToLower();
                return Contient(a.Stock?.Asset, filtre)
                    || Contient(a.Stock?.NumSerie, filtre)
                    || Contient(a.Stock?.Modele?.Nom, filtre)
                    || Contient(a.Utilisateur?.Nom, filtre)
                    || Contient(a.Utilisateur?.Prenom, filtre)
                    || Contient(a.Utilisateur?.IdWindows, filtre)
                    || Contient(a.Eds?.Nom, filtre)
                    || Contient(a.Eds?.Cnx, filtre);
            };
            view.Refresh();
        }

        private void AppliquerFiltreStocksDisponibles()
        {
            var view = CollectionViewSource.GetDefaultView(StocksDisponibles);
            if (view == null)
                return;

            view.Filter = o =>
            {
                if (o is not Stock s)
                    return false;

                if (string.IsNullOrWhiteSpace(FiltreMateriel))
                    return true;

                var filtre = FiltreMateriel.Trim().ToLower();
                return Contient(s.Asset, filtre)
                    || Contient(s.NumSerie, filtre)
                    || Contient(s.Modele?.Nom, filtre)
                    || Contient(s.Modele?.Marque?.Nom, filtre)
                    || Contient(s.Modele?.Materiel?.Nom, filtre);
            };
            view.Refresh();
        }

        private void FiltrerForfaits()
        {
            var view = CollectionViewSource.GetDefaultView(Forfaits);
            if (view == null)
                return;

            view.Filter = o => o is Forfait f &&
                (OperateurSelectionne == null || f.OperateurId == OperateurSelectionne.Id);
            view.Refresh();
        }

        private async Task RechercherUtilisateursAsync()
        {
            if (string.IsNullOrWhiteSpace(FiltreUtilisateur) || FiltreUtilisateur.Trim().Length < 2)
            {
                App.Current.Dispatcher.Invoke(Utilisateurs.Clear);
                return;
            }

            var recherche = FiltreUtilisateur;
            var utilisateurs = await _utilisateurRepository.RechercherPourAffectation(recherche);

            if (!string.Equals(recherche, FiltreUtilisateur, StringComparison.Ordinal))
                return;

            App.Current.Dispatcher.Invoke(() => Remplacer(Utilisateurs, utilisateurs));
        }

        private async Task RechercherEdsAsync()
        {
            if (string.IsNullOrWhiteSpace(FiltreEds) || FiltreEds.Trim().Length < 2)
            {
                App.Current.Dispatcher.Invoke(EdsListe.Clear);
                return;
            }

            var recherche = FiltreEds;
            var eds = await _edsRepository.RechercherLight(recherche);

            if (!string.Equals(recherche, FiltreEds, StringComparison.Ordinal))
                return;

            App.Current.Dispatcher.Invoke(() => Remplacer(EdsListe, eds));
        }

        private async Task SelectionnerEdsDepuisUtilisateurAsync()
        {
            if (_resetEnCours || UtilisateurSelectionne == null)
            {
                EdsAutomatiqueSelectionne = null;
                EdsSelectionne = null;
                return;
            }

            var service = UtilisateurSelectionne?.Departement;
            if (string.IsNullOrWhiteSpace(service))
                return;

            var cible = ExtraireCibleDepuisDepartement(service);
            if (string.IsNullOrWhiteSpace(cible))
                return;

            var eds = await _edsLiaisonRepository.GetEdsParService(cible);
            if (_resetEnCours || UtilisateurSelectionne == null)
                return;

            if (eds == null)
            {
                MessageSucces = string.Empty;
                ErreurGlobale = $"Aucun EDS trouvé pour la cible : {cible}";
                return;
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                if (!EdsListe.Any(e => e.Id == eds.Id))
                    EdsListe.Add(eds);

                var edsDansListe = EdsListe.First(e => e.Id == eds.Id);
                EdsAutomatiqueSelectionne = edsDansListe;
                EdsSelectionne = edsDansListe;
                _filtreEds = string.Empty;
                OnPropertyChanged(nameof(FiltreEds));
                ErreurGlobale = string.Empty;
            });
        }

        private static string ExtraireCibleDepuisDepartement(string departement)
        {
            var valeur = departement.Trim();
            if (valeur.Length <= 5)
                return valeur;

            return valeur[^5..];
        }

        private static bool Contient(string? valeur, string filtre)
        {
            return valeur?.ToLower().Contains(filtre) == true;
        }

        private static void Remplacer<T>(ObservableCollection<T> cible, IEnumerable<T> valeurs)
        {
            cible.Clear();
            foreach (var valeur in valeurs)
                cible.Add(valeur);
        }

        private void ChargerFormulaireDepuisAffectation(Affectation affectation)
        {
            if (affectation.Stock != null && !StocksDisponibles.Any(s => s.Id == affectation.Stock.Id))
                StocksDisponibles.Add(affectation.Stock);

            if (affectation.Utilisateur != null && !Utilisateurs.Any(u => u.Id == affectation.Utilisateur.Id))
                Utilisateurs.Add(affectation.Utilisateur);

            if (affectation.Eds != null && !EdsListe.Any(e => e.Id == affectation.Eds.Id))
                EdsListe.Add(affectation.Eds);

            if (affectation.EdsAutomatique != null && !EdsListe.Any(e => e.Id == affectation.EdsAutomatique.Id))
                EdsListe.Add(affectation.EdsAutomatique);

            _filtreMateriel = string.Empty;
            OnPropertyChanged(nameof(FiltreMateriel));
            AppliquerFiltreStocksDisponibles();
            StockSelectionne = StocksDisponibles.FirstOrDefault(s => s.Id == affectation.StockId);
            StatutAffectationSelectionne = StatutsAffectation.FirstOrDefault(s => s.Id == affectation.Stock?.StatutId);

            _utilisateurSelectionne = Utilisateurs.FirstOrDefault(u => u.Id == affectation.UtilisateurId);
            OnPropertyChanged(nameof(UtilisateurSelectionne));

            EdsAutomatiqueSelectionne = affectation.EdsAutomatiqueId == null
                ? null
                : EdsListe.FirstOrDefault(e => e.Id == affectation.EdsAutomatiqueId);

            EdsSelectionne = affectation.EdsId == null
                ? null
                : EdsListe.FirstOrDefault(e => e.Id == affectation.EdsId);

            OperateurSelectionne = affectation.OperateurId == null
                ? null
                : Operateurs.FirstOrDefault(o => o.Id == affectation.OperateurId);

            ForfaitSelectionne = affectation.ForfaitId == null
                ? null
                : Forfaits.FirstOrDefault(f => f.Id == affectation.ForfaitId);

            DateDebut = affectation.DateDebut;
            DateFin = affectation.DateFin;
            NomAppareil = affectation.NomAppareil;
            AdresseIP = affectation.AdresseIP;
            MasqueIP = affectation.MasqueIP;
            PasserelleIP = affectation.PasserelleIP;
            NomPC = affectation.NomPC;
            EdsPC = affectation.EdsPC;
            AncienPC = affectation.AncienPC;
            NumTelMobile = affectation.NumTelMobile;
            Motif = affectation.Motif;
            Commentaire = affectation.Commentaire;
            DateMouvement = DateTime.Today;
            StatutRetourSelectionne = GetStatutRetourDefaut();
            MotifRetour = string.Empty;
            CommentaireRetour = string.Empty;
            _filtreUtilisateur = string.Empty;
            _filtreEds = string.Empty;
            OnPropertyChanged(nameof(FiltreUtilisateur));
            OnPropertyChanged(nameof(FiltreEds));
            OnPropertyChanged(nameof(EstModification));
            OnPropertyChanged(nameof(PeutAjouter));
            OnPropertyChanged(nameof(PeutModifier));
            OnPropertyChanged(nameof(PeutRetourner));
            OnPropertyChanged(nameof(PeutChoisirMateriel));
            CommandManager.InvalidateRequerySuggested();
        }

        private bool ValiderRetour()
        {
            if (StatutRetourSelectionne == null)
            {
                ErreurGlobale = "Le statut de retour est obligatoire.";
                return false;
            }

            return true;
        }

        private Statut? GetStatutRetourDefaut()
        {
            return StatutsRetour.FirstOrDefault(s => MemeNomStatut(s.Nom, "stock"));
        }

        private void ActualiserStatutsAffectationDisponibles()
        {
            var statuts = EstModification
                ? _tousStatutsAffectation
                : _tousStatutsAffectation.Where(EstStatutCreationAffectation).ToList();

            var statutSelectionneId = StatutAffectationSelectionne?.Id;
            Remplacer(StatutsAffectation, statuts);

            StatutAffectationSelectionne = statutSelectionneId == null
                ? null
                : StatutsAffectation.FirstOrDefault(s => s.Id == statutSelectionneId);

            SelectionnerStatutParDefaut();
        }

        private bool ValiderStatutAffectation()
        {
            if (StatutAffectationSelectionne == null)
            {
                ErreurGlobale = "Le statut d'affectation est obligatoire.";
                return false;
            }

            if (DateFinObligatoire && DateFin == null)
            {
                ErreurGlobale = "La date de fin est obligatoire pour le statut Pret.";
                return false;
            }

            return true;
        }

        private void SelectionnerStatutParDefaut()
        {
            if (EstModification || StockSelectionne == null || StatutsAffectation.Count == 0)
                return;

            var nomStatut = EstOrdinateurPortable() ? "personnalise" : "installe";
            StatutAffectationSelectionne = StatutsAffectation.FirstOrDefault(s => MemeNomStatut(s.Nom, nomStatut));
        }

        private bool EstOrdinateurPortable()
        {
            var materiel = StockSelectionne?.Modele?.Materiel?.Nom;
            if (string.IsNullOrWhiteSpace(materiel))
                return false;

            return materiel.Contains("ordinateur", StringComparison.OrdinalIgnoreCase)
                && materiel.Contains("portable", StringComparison.OrdinalIgnoreCase);
        }

        private bool EstStatutSelectionne(string nom)
        {
            return StatutAffectationSelectionne != null && MemeNomStatut(StatutAffectationSelectionne.Nom, nom);
        }

        private static bool EstStatutCreationAffectation(Statut statut)
        {
            return MemeNomStatut(statut.Nom, "installe")
                || MemeNomStatut(statut.Nom, "personnalise")
                || MemeNomStatut(statut.Nom, "pret")
                || MemeNomStatut(statut.Nom, "tiers");
        }

        private static bool MemeNomStatut(string? valeur, string attendu)
        {
            return string.Equals(SansAccents(valeur), attendu, StringComparison.OrdinalIgnoreCase);
        }

        private static string SansAccents(string? valeur)
        {
            if (string.IsNullOrWhiteSpace(valeur))
                return string.Empty;

            var normalise = valeur.Normalize(System.Text.NormalizationForm.FormD);
            return new string(normalise
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray())
                .Normalize(System.Text.NormalizationForm.FormC);
        }
    }
}
