using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;
using System.Globalization;
using System.IO;

namespace Stock741.Repositories
{
    public class UtilisateurRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UtilisateurRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        //public async Task<List<Utilisateur>> GetAll()
        //{
        //    using var context = _contextFactory.CreateDbContext();
        //    return await context.Utilisateurs
        //        .AsNoTracking()
        //        .OrderBy(u => u.Nom)
        //        .ThenBy(u => u.Prenom)
        //        .ToListAsync();
        //}

        public async Task<List<Utilisateur>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Utilisateurs
                .AsNoTracking()
                .OrderBy(u => u.Nom)
                .ThenBy(u => u.Prenom)
                .Select(u => new Utilisateur
                {
                    Id = u.Id,
                    IdWindows = u.IdWindows,
                    Nom = u.Nom,
                    Prenom = u.Prenom,
                    Email = u.Email,
                    Departement = u.Departement,
                    Emplacement = u.Emplacement,
                    Actif = u.Actif
                })
                .ToListAsync();
        }

        public async Task<List<Utilisateur>> RechercherPourAffectation(string recherche, int limite = 50)
        {
            if (string.IsNullOrWhiteSpace(recherche) || recherche.Trim().Length < 2)
                return new List<Utilisateur>();

            var filtre = recherche.Trim().ToLower();

            using var context = _contextFactory.CreateDbContext();
            return await context.Utilisateurs
                .AsNoTracking()
                .Where(u =>
                    u.Nom.ToLower().Contains(filtre) ||
                    u.Prenom.ToLower().Contains(filtre) ||
                    u.IdWindows.ToLower().Contains(filtre))
                .OrderBy(u => u.Nom)
                .ThenBy(u => u.Prenom)
                .Take(limite)
                .Select(u => new Utilisateur
                {
                    Id = u.Id,
                    IdWindows = u.IdWindows,
                    Nom = u.Nom,
                    Prenom = u.Prenom,
                    NomComplet = u.NomComplet,
                    Email = u.Email,
                    Departement = u.Departement,
                    Emplacement = u.Emplacement,
                    Actif = u.Actif
                })
                .ToListAsync();
        }

        public async Task<Utilisateur> GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Utilisateurs
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<(int inseres, int misAJour)> ImporterCsv(string cheminFichier, IProgress<(int traites, int total)> progress = null)
        {
            var config = new CsvConfiguration(new CultureInfo("fr-FR"))
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                BadDataFound = null,
                Encoding = System.Text.Encoding.GetEncoding("iso-8859-1")
            };

            var utilisateursCsv = new List<Utilisateur>();

            using (var reader = new StreamReader(cheminFichier, System.Text.Encoding.GetEncoding("iso-8859-1")))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<UtilisateurCsvMap>();
                utilisateursCsv = csv.GetRecords<Utilisateur>().ToList();
            }

            int inseres = 0;
            int misAJour = 0;
            int traites = 0;
            int total = utilisateursCsv.Count;

            progress?.Report((0, total));

            using var context = _contextFactory.CreateDbContext();

            foreach (var utilisateurCsv in utilisateursCsv)
            {
                if (string.IsNullOrWhiteSpace(utilisateurCsv.IdWindows))
                {
                    traites++;
                    progress?.Report((traites, total));
                    continue;
                }

                var existant = await context.Utilisateurs
                    .FirstOrDefaultAsync(u => u.IdWindows == utilisateurCsv.IdWindows);

                if (existant == null)
                {
                    context.Utilisateurs.Add(utilisateurCsv);
                    inseres++;
                }
                else
                {
                    existant.Societe = utilisateurCsv.Societe;
                    existant.IdUtilisateur = utilisateurCsv.IdUtilisateur;
                    existant.Prenom = utilisateurCsv.Prenom;
                    existant.Nom = utilisateurCsv.Nom;
                    existant.NomComplet = utilisateurCsv.NomComplet;
                    existant.TelephoneMobile = utilisateurCsv.TelephoneMobile;
                    existant.TelephoneProfessionnel = utilisateurCsv.TelephoneProfessionnel;
                    existant.Email = utilisateurCsv.Email;
                    existant.Emplacement = utilisateurCsv.Emplacement;
                    existant.Departement = utilisateurCsv.Departement;
                    existant.Bureau = utilisateurCsv.Bureau;
                    existant.Rue = utilisateurCsv.Rue;
                    existant.CodePostal = utilisateurCsv.CodePostal;
                    existant.Ville = utilisateurCsv.Ville;
                    existant.CodePays = utilisateurCsv.CodePays;
                    existant.Vip = utilisateurCsv.Vip;
                    existant.Actif = utilisateurCsv.Actif;
                    existant.Manager = utilisateurCsv.Manager;
                    existant.FuseauHoraire = utilisateurCsv.FuseauHoraire;
                    existant.DateCreation = utilisateurCsv.DateCreation;
                    existant.CreePar = utilisateurCsv.CreePar;
                    existant.MisAJourPar = utilisateurCsv.MisAJourPar;
                    existant.DateMiseAJour = utilisateurCsv.DateMiseAJour;
                    misAJour++;
                }

                traites++;
                progress?.Report((traites, total));
            }

            await context.SaveChangesAsync();
            return (inseres, misAJour);
        }
    }

    public class UtilisateurCsvMap : ClassMap<Utilisateur>
    {
        public UtilisateurCsvMap()
        {
            Map(u => u.Societe).Name("Société");
            Map(u => u.IdUtilisateur).Name("ID Utilisateur");
            Map(u => u.Prenom).Name("Prénom");
            Map(u => u.Nom).Name("Nom");
            Map(u => u.NomComplet).Name("Nom Complet");
            Map(u => u.TelephoneMobile).Name("Téléphone Mobile");
            Map(u => u.TelephoneProfessionnel).Name("Téléphone Professionnel");
            Map(u => u.Email).Name("E-mail");
            Map(u => u.Emplacement).Name("Emplacement");
            Map(u => u.Departement).Name("Département");
            Map(u => u.Bureau).Name("Bureau");
            Map(u => u.Rue).Name("Rue");
            Map(u => u.CodePostal).Name("Code postal");
            Map(u => u.Ville).Name("Ville");
            Map(u => u.CodePays).Name("Code du pays");
            Map(u => u.IdWindows).Name("ID Windows");
            Map(u => u.Vip).Name("VIP").Convert(row =>
            {
                var val = row.Row.GetField("VIP")?.Trim().ToLower();
                return val == "vrai" || val == "true" || val == "1" || val == "oui";
            });
            Map(u => u.Actif).Name("Actif").Convert(row =>
            {
                var val = row.Row.GetField("Actif")?.Trim().ToLower();
                return val == "vrai" || val == "true" || val == "1" || val == "oui";
            });
            Map(u => u.Manager).Name("Manager");
            Map(u => u.FuseauHoraire).Name("Fuseau horaire");
            Map(u => u.DateCreation).Name("Date de création").Convert(row =>
            {
                var val = row.Row.GetField("Date de création")?.Trim();
                if (string.IsNullOrWhiteSpace(val)) return (DateTime?)null;
                if (DateTime.TryParseExact(val, "dd/MM/yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    return date;
                return (DateTime?)null;
            });
            Map(u => u.CreePar).Name("Créé par");
            Map(u => u.MisAJourPar).Name("Mis à jour par");
            Map(u => u.DateMiseAJour).Name("Date de mise à jour").Convert(row =>
            {
                var val = row.Row.GetField("Date de mise à jour")?.Trim();
                if (string.IsNullOrWhiteSpace(val)) return (DateTime?)null;
                if (DateTime.TryParseExact(val, "dd/MM/yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    return date;
                return (DateTime?)null;
            });
        }
    }
}
