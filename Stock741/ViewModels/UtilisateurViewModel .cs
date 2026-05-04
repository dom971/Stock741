using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Stock741.Commands;
using Stock741.Models;
using Stock741.Repositories;

namespace Stock741.ViewModels
{
    public class UtilisateurViewModel : BaseViewModel
    {
        private readonly UtilisateurRepository _repository;

        public ObservableCollection<Utilisateur> Utilisateurs { get; set; }

        private Utilisateur _utilisateurSelectionne;
        public Utilisateur UtilisateurSelectionne
        {
            get => _utilisateurSelectionne;
            set
            {
                _utilisateurSelectionne = value;
                OnPropertyChanged();
                if (value != null)
                    _ = ChargerDetailAsync(value.Id);
            }
        }

        private Utilisateur _detail;
        public Utilisateur Detail
        {
            get => _detail;
            set { _detail = value; OnPropertyChanged(); }
        }

        private string _filtreNom = string.Empty;
        public string FiltreNom
        {
            get => _filtreNom;
            set { _filtreNom = value; OnPropertyChanged(); AppliquerFiltre(); }
        }

        private string _erreurGlobale;
        public string ErreurGlobale
        {
            get => _erreurGlobale;
            set { _erreurGlobale = value; OnPropertyChanged(); }
        }

        private string _messageSucces;
        public string MessageSucces
        {
            get => _messageSucces;
            set { _messageSucces = value; OnPropertyChanged(); }
        }

        private bool _chargement;
        public bool Chargement
        {
            get => _chargement;
            set { _chargement = value; OnPropertyChanged(); }
        }

        private int _progression;
        public int Progression
        {
            get => _progression;
            set { _progression = value; OnPropertyChanged(); }
        }

        private int _totalEnregistrements;
        public int TotalEnregistrements
        {
            get => _totalEnregistrements;
            set { _totalEnregistrements = value; OnPropertyChanged(); }
        }

        public ICommand ImporterCsvCommand { get; }
        public ICommand ActualiserCommand { get; }

        public UtilisateurViewModel(UtilisateurRepository repository)
        {
            _repository = repository;
            Utilisateurs = new ObservableCollection<Utilisateur>();

            ImporterCsvCommand = new AsyncRelayCommand(ImporterCsv);
            ActualiserCommand = new AsyncRelayCommand(async _ =>
            {
                await Rafraichir();
                EffacerErreur();
            });
        }

        private void AppliquerFiltre()
        {
            var view = System.Windows.Data.CollectionViewSource.GetDefaultView(Utilisateurs);
            if (view != null)
                view.Filter = o => o is Utilisateur u &&
                    (string.IsNullOrWhiteSpace(FiltreNom) ||
                     (u.Nom?.ToLower().Contains(FiltreNom.ToLower()) ?? false) ||
                     (u.Prenom?.ToLower().Contains(FiltreNom.ToLower()) ?? false) ||
                     (u.IdWindows?.ToLower().Contains(FiltreNom.ToLower()) ?? false));
        }

        private async Task ChargerDetailAsync(int id)
        {
            var utilisateur = await _repository.GetById(id);
            Detail = utilisateur;
        }

        public async Task Rafraichir()
        {
            var liste = await _repository.GetAll();
            App.Current.Dispatcher.Invoke(() =>
            {
                Utilisateurs.Clear();
                foreach (var u in liste)
                    Utilisateurs.Add(u);
                AppliquerFiltre();
            });
            Detail = null;
            _utilisateurSelectionne = null;
            OnPropertyChanged(nameof(UtilisateurSelectionne));
        }

        public void EffacerErreur()
        {
            ErreurGlobale = string.Empty;
            MessageSucces = string.Empty;
        }

        private async Task ImporterCsv(object obj)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Fichiers CSV|*.csv",
                Title = "Sélectionner le fichier utilisateurs"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                Chargement = true;
                Progression = 0;
                TotalEnregistrements = 0;
                ErreurGlobale = string.Empty;
                MessageSucces = string.Empty;

                var progress = new Progress<(int traites, int total)>(valeur =>
                {
                    TotalEnregistrements = valeur.total;
                    Progression = valeur.traites;
                });

                var (inseres, misAJour) = await Task.Run(() =>
                    _repository.ImporterCsv(dialog.FileName, progress));

                await Rafraichir();
                MessageSucces = $"Import terminé — {inseres} inseré(s), {misAJour} mis à jour.";
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                if (ex.InnerException != null)
                    message += " | " + ex.InnerException.Message;
                if (ex.InnerException?.InnerException != null)
                    message += " | " + ex.InnerException.InnerException.Message;
                ErreurGlobale = $"Erreur lors de l'import : {message}";
            }
            finally
            {
                Chargement = false;
            }
        }
    }
}