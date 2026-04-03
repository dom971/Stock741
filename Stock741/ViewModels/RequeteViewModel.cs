using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Stock741.Commands;
using Stock741.Data;

namespace Stock741.ViewModels
{
    public class RequeteViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;

        private string _requete = string.Empty;
        public string Requete
        {
            get => _requete;
            set { _requete = value; OnPropertyChanged(); }
        }

        private DataView _resultats;
        public DataView Resultats
        {
            get => _resultats;
            set { _resultats = value; OnPropertyChanged(); }
        }

        private string _erreurGlobale;
        public string ErreurGlobale
        {
            get => _erreurGlobale;
            set { _erreurGlobale = value; OnPropertyChanged(); }
        }

        private string _infoResultat;
        public string InfoResultat
        {
            get => _infoResultat;
            set { _infoResultat = value; OnPropertyChanged(); }
        }

        public ICommand ExecuterRequeteCommand { get; }
        public ICommand EffacerCommand { get; }

        public RequeteViewModel(AppDbContext context)
        {
            _context = context;
            ExecuterRequeteCommand = new RelayCommand(ExecuterRequete);
            EffacerCommand = new RelayCommand(Effacer);
        }

        private void ExecuterRequete(object obj)
        {
            ErreurGlobale = string.Empty;
            InfoResultat = string.Empty;
            Resultats = null;

            if (string.IsNullOrWhiteSpace(Requete))
            {
                ErreurGlobale = "Veuillez saisir une requête.";
                return;
            }

            var requeteNormalisee = Requete.Trim().ToUpper();
            if (!requeteNormalisee.StartsWith("SELECT"))
            {
                ErreurGlobale = "Seules les requêtes SELECT sont autorisées.";
                return;
            }

            // Bloquer les mots-clés dangereux
            string[] motsInterdits = { "INSERT", "UPDATE", "DELETE", "DROP", "ALTER", "CREATE", "TRUNCATE", "EXEC", "EXECUTE" };
            foreach (var mot in motsInterdits)
            {
                if (requeteNormalisee.Contains(mot))
                {
                    ErreurGlobale = $"Mot-clé interdit détecté : {mot}";
                    return;
                }
            }

            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();

                using var commande = connexion.CreateCommand();
                commande.CommandText = Requete;

                using var reader = commande.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);

                Resultats = table.DefaultView;
                InfoResultat = $"{table.Rows.Count} ligne(s) retournée(s).";

                connexion.Close();
            }
            catch (Exception ex)
            {
                ErreurGlobale = $"Erreur SQL : {ex.Message}";
            }
        }

        private void Effacer(object obj)
        {
            Requete = string.Empty;
            Resultats = null;
            ErreurGlobale = string.Empty;
            InfoResultat = string.Empty;
        }
    }
}
