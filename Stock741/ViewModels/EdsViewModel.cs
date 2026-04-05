using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class EdsViewModel : BaseViewModel
    {
        private readonly EdsRepository _repository;

        public ObservableCollection<Eds> EdsList { get; set; }

        private string _cnxSelectionne;
        public string CnxSelectionne
        {
            get => _cnxSelectionne;
            set { _cnxSelectionne = value; OnPropertyChanged(); ValidateCnx(); }
        }

        private string _nomSelectionne;
        public string NomSelectionne
        {
            get => _nomSelectionne;
            set { _nomSelectionne = value; OnPropertyChanged(); ValidateNom(); }
        }

        private string _adr1Selectionne;
        public string Adr1Selectionne
        {
            get => _adr1Selectionne;
            set { _adr1Selectionne = value; OnPropertyChanged(); }
        }

        private string _adr2Selectionne;
        public string Adr2Selectionne
        {
            get => _adr2Selectionne;
            set { _adr2Selectionne = value; OnPropertyChanged(); }
        }

        private string _adr3Selectionne;
        public string Adr3Selectionne
        {
            get => _adr3Selectionne;
            set { _adr3Selectionne = value; OnPropertyChanged(); }
        }

        private string _adr4Selectionne;
        public string Adr4Selectionne
        {
            get => _adr4Selectionne;
            set { _adr4Selectionne = value; OnPropertyChanged(); }
        }

        private string _horLundiSelectionne;
        public string HorLundiSelectionne
        {
            get => _horLundiSelectionne;
            set { _horLundiSelectionne = value; OnPropertyChanged(); }
        }

        private string _horMardiSelectionne;
        public string HorMardiSelectionne
        {
            get => _horMardiSelectionne;
            set { _horMardiSelectionne = value; OnPropertyChanged(); }
        }

        private string _horMercrediSelectionne;
        public string HorMercrediSelectionne
        {
            get => _horMercrediSelectionne;
            set { _horMercrediSelectionne = value; OnPropertyChanged(); }
        }

        private string _horJeudiSelectionne;
        public string HorJeudiSelectionne
        {
            get => _horJeudiSelectionne;
            set { _horJeudiSelectionne = value; OnPropertyChanged(); }
        }

        private string _horVendrediSelectionne;
        public string HorVendrediSelectionne
        {
            get => _horVendrediSelectionne;
            set { _horVendrediSelectionne = value; OnPropertyChanged(); }
        }

        private string _horSamediSelectionne;
        public string HorSamediSelectionne
        {
            get => _horSamediSelectionne;
            set { _horSamediSelectionne = value; OnPropertyChanged(); }
        }

        private string _geolocalisationSelectionne;
        public string GeolocalisationSelectionne
        {
            get => _geolocalisationSelectionne;
            set { _geolocalisationSelectionne = value; OnPropertyChanged(); }
        }

        private string _mailContactSelectionne;
        public string MailContactSelectionne
        {
            get => _mailContactSelectionne;
            set { _mailContactSelectionne = value; OnPropertyChanged(); }
        }

        private bool _actifSelectionne = true;
        public bool ActifSelectionne
        {
            get => _actifSelectionne;
            set { _actifSelectionne = value; OnPropertyChanged(); }
        }

        private Eds _edsSelectionne;
        public Eds EdsSelectionne
        {
            get => _edsSelectionne;
            set
            {
                _edsSelectionne = value;
                OnPropertyChanged();
                if (value != null)
                {
                    CnxSelectionne = value.Cnx;
                    NomSelectionne = value.Nom;
                    Adr1Selectionne = value.Adr1;
                    Adr2Selectionne = value.Adr2;
                    Adr3Selectionne = value.Adr3;
                    Adr4Selectionne = value.Adr4;
                    HorLundiSelectionne = value.HorLundi;
                    HorMardiSelectionne = value.HorMardi;
                    HorMercrediSelectionne = value.HorMercredi;
                    HorJeudiSelectionne = value.HorJeudi;
                    HorVendrediSelectionne = value.HorVendredi;
                    HorSamediSelectionne = value.HorSamedi;
                    GeolocalisationSelectionne = value.Geolocalisation;
                    MailContactSelectionne = value.MailContact;
                    ActifSelectionne = value.Actif;
                }
            }
        }

        private string _erreurCnx;
        public string ErreurCnx
        {
            get => _erreurCnx;
            set { _erreurCnx = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        private string _erreurNom;
        public string ErreurNom
        {
            get => _erreurNom;
            set { _erreurNom = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasErreur)); }
        }

        public bool HasErreur => !string.IsNullOrWhiteSpace(ErreurCnx) ||
                                  !string.IsNullOrWhiteSpace(ErreurNom);

        private string _erreurGlobale;
        public string ErreurGlobale
        {
            get => _erreurGlobale;
            set { _erreurGlobale = value; OnPropertyChanged(); }
        }

        public ICommand AjouterEdsCommand { get; }
        public ICommand ModifierEdsCommand { get; }
        public ICommand SupprimerEdsCommand { get; }
        public ICommand InitialiserEdsCommand { get; }

        public EdsViewModel(EdsRepository repository)
        {
            _repository = repository;
            EdsList = new ObservableCollection<Eds>(_repository.GetAll());

            AjouterEdsCommand = new RelayCommand(AjouterEds);
            ModifierEdsCommand = new RelayCommand(ModifierEds);
            SupprimerEdsCommand = new RelayCommand(SupprimerEds);
            InitialiserEdsCommand = new RelayCommand(InitialiserEds);
        }

        private void ValidateCnx()
        {
            if (string.IsNullOrWhiteSpace(CnxSelectionne))
                ErreurCnx = "Code obligatoire";
            else if (EdsList.Any(e => e.Cnx.ToLower() == CnxSelectionne.ToLower() &&
                                      (EdsSelectionne == null || e.Id != EdsSelectionne.Id)))
                ErreurCnx = "Code déjà utilisé";
            else
                ErreurCnx = string.Empty;
        }

        private void ValidateNom()
        {
            if (string.IsNullOrWhiteSpace(NomSelectionne))
                ErreurNom = "Nom obligatoire";
            else
                ErreurNom = string.Empty;
        }

        private void ResetChamps()
        {
            CnxSelectionne = string.Empty;
            NomSelectionne = string.Empty;
            Adr1Selectionne = string.Empty;
            Adr2Selectionne = string.Empty;
            Adr3Selectionne = string.Empty;
            Adr4Selectionne = string.Empty;
            HorLundiSelectionne = string.Empty;
            HorMardiSelectionne = string.Empty;
            HorMercrediSelectionne = string.Empty;
            HorJeudiSelectionne = string.Empty;
            HorVendrediSelectionne = string.Empty;
            HorSamediSelectionne = string.Empty;
            GeolocalisationSelectionne = string.Empty;
            MailContactSelectionne = string.Empty;
            ActifSelectionne = true;
            ErreurGlobale = string.Empty;
        }

        private void InitialiserEds(object obj)
        {
            EdsSelectionne = null;
            ResetChamps();
        }

        private void AjouterEds(object obj)
        {
            ValidateCnx();
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurCnx + " " + ErreurNom;
                return;
            }

            var eds = new Eds
            {
                Cnx = CnxSelectionne,
                Nom = NomSelectionne,
                Adr1 = Adr1Selectionne,
                Adr2 = Adr2Selectionne,
                Adr3 = Adr3Selectionne,
                Adr4 = Adr4Selectionne,
                HorLundi = HorLundiSelectionne,
                HorMardi = HorMardiSelectionne,
                HorMercredi = HorMercrediSelectionne,
                HorJeudi = HorJeudiSelectionne,
                HorVendredi = HorVendrediSelectionne,
                HorSamedi = HorSamediSelectionne,
                Geolocalisation = GeolocalisationSelectionne,
                MailContact = MailContactSelectionne,
                Actif = ActifSelectionne
            };

            try
            {
                _repository.Add(eds);
                EdsList.Add(eds);
                ResetChamps();
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
            }
        }

        private void ModifierEds(object obj)
        {
            if (EdsSelectionne == null) return;
            ValidateCnx();
            ValidateNom();
            if (HasErreur)
            {
                ErreurGlobale = ErreurCnx + " " + ErreurNom;
                return;
            }

            var ancienCnx = EdsSelectionne.Cnx;
            var ancienNom = EdsSelectionne.Nom;
            var ancienAdr1 = EdsSelectionne.Adr1;
            var ancienAdr2 = EdsSelectionne.Adr2;
            var ancienAdr3 = EdsSelectionne.Adr3;
            var ancienAdr4 = EdsSelectionne.Adr4;
            var ancienHorLundi = EdsSelectionne.HorLundi;
            var ancienHorMardi = EdsSelectionne.HorMardi;
            var ancienHorMercredi = EdsSelectionne.HorMercredi;
            var ancienHorJeudi = EdsSelectionne.HorJeudi;
            var ancienHorVendredi = EdsSelectionne.HorVendredi;
            var ancienHorSamedi = EdsSelectionne.HorSamedi;
            var ancienGeo = EdsSelectionne.Geolocalisation;
            var ancienMail = EdsSelectionne.MailContact;
            var ancienActif = EdsSelectionne.Actif;

            EdsSelectionne.Cnx = CnxSelectionne;
            EdsSelectionne.Nom = NomSelectionne;
            EdsSelectionne.Adr1 = Adr1Selectionne;
            EdsSelectionne.Adr2 = Adr2Selectionne;
            EdsSelectionne.Adr3 = Adr3Selectionne;
            EdsSelectionne.Adr4 = Adr4Selectionne;
            EdsSelectionne.HorLundi = HorLundiSelectionne;
            EdsSelectionne.HorMardi = HorMardiSelectionne;
            EdsSelectionne.HorMercredi = HorMercrediSelectionne;
            EdsSelectionne.HorJeudi = HorJeudiSelectionne;
            EdsSelectionne.HorVendredi = HorVendrediSelectionne;
            EdsSelectionne.HorSamedi = HorSamediSelectionne;
            EdsSelectionne.Geolocalisation = GeolocalisationSelectionne;
            EdsSelectionne.MailContact = MailContactSelectionne;
            EdsSelectionne.Actif = ActifSelectionne;

            try
            {
                _repository.Update(EdsSelectionne);
                CollectionViewSource.GetDefaultView(EdsList).Refresh();
                ErreurGlobale = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                EdsSelectionne.Cnx = ancienCnx;
                EdsSelectionne.Nom = ancienNom;
                EdsSelectionne.Adr1 = ancienAdr1;
                EdsSelectionne.Adr2 = ancienAdr2;
                EdsSelectionne.Adr3 = ancienAdr3;
                EdsSelectionne.Adr4 = ancienAdr4;
                EdsSelectionne.HorLundi = ancienHorLundi;
                EdsSelectionne.HorMardi = ancienHorMardi;
                EdsSelectionne.HorMercredi = ancienHorMercredi;
                EdsSelectionne.HorJeudi = ancienHorJeudi;
                EdsSelectionne.HorVendredi = ancienHorVendredi;
                EdsSelectionne.HorSamedi = ancienHorSamedi;
                EdsSelectionne.Geolocalisation = ancienGeo;
                EdsSelectionne.MailContact = ancienMail;
                EdsSelectionne.Actif = ancienActif;
                ErreurGlobale = ex.Message;
            }
        }

        private void SupprimerEds(object obj)
        {
            if (EdsSelectionne == null) return;

            try
            {
                _repository.Delete(EdsSelectionne);
                EdsList.Remove(EdsSelectionne);
                EdsSelectionne = null;
                ResetChamps();
            }
            catch (InvalidOperationException ex)
            {
                ErreurGlobale = ex.Message;
                EdsSelectionne = null;
                ResetChamps();
            }
        }
    }
}