using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class StockRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public StockRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Stock>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await ConstruireRequeteStock(context.Stocks.AsNoTracking(), context)
                .OrderByDescending(s => s.Date)
                .Take(200)
                .ToListAsync();
        }

        public async Task<List<Stock>> Rechercher(string recherche, int limite = 200)
        {
            if (string.IsNullOrWhiteSpace(recherche) || recherche.Trim().Length < 2)
                return await GetAll();

            var filtre = recherche.Trim().ToLower();

            using var context = _contextFactory.CreateDbContext();
            var requete = context.Stocks
                .AsNoTracking()
                .Where(s =>
                    (s.Asset != null && s.Asset.ToLower().Contains(filtre)) ||
                    s.NumSerie.ToLower().Contains(filtre) ||
                    s.Modele.Nom.ToLower().Contains(filtre) ||
                    s.Modele.Materiel.Nom.ToLower().Contains(filtre) ||
                    s.Modele.Marque.Nom.ToLower().Contains(filtre) ||
                    (s.Fournisseur != null && s.Fournisseur.Nom.ToLower().Contains(filtre)) ||
                    (s.Lieu != null && s.Lieu.Nom.ToLower().Contains(filtre)));

            return await ConstruireRequeteStock(requete, context)
                .OrderByDescending(s => s.Date)
                .Take(limite)
                .ToListAsync();
        }

        private static IQueryable<Stock> ConstruireRequeteStock(IQueryable<Stock> requete, AppDbContext context)
        {
            return requete
                .AsNoTracking()
                .Select(s => new Stock
                {
                    Id = s.Id,
                    Asset = s.Asset,
                    Date = s.Date,
                    DateMouvement = s.DateMouvement,
                    NumSerie = s.NumSerie,
                    StatutId = s.StatutId,
                    Statut = s.Statut == null ? null : new Statut
                    {
                        Id = s.Statut.Id,
                        Nom = s.Statut.Nom,
                        Type = s.Statut.Type
                    },
                    LieuId = s.LieuId,
                    Lieu = s.Lieu == null ? null : new Lieu
                    {
                        Id = s.Lieu.Id,
                        Nom = s.Lieu.Nom
                    },
                    ModeleId = s.ModeleId,
                    Modele = new Modele
                    {
                        Id = s.Modele.Id,
                        Nom = s.Modele.Nom,
                        CheminPhoto = s.Modele.CheminPhoto,
                        MaterielId = s.Modele.MaterielId,
                        Materiel = new Materiel
                        {
                            Id = s.Modele.Materiel.Id,
                            Nom = s.Modele.Materiel.Nom
                        },
                        MarqueId = s.Modele.MarqueId,
                        Marque = new Marque
                        {
                            Id = s.Modele.Marque.Id,
                            Nom = s.Modele.Marque.Nom
                        }
                    },
                    FournisseurId = s.FournisseurId,
                    Fournisseur = s.Fournisseur == null ? null : new Fournisseur
                    {
                        Id = s.Fournisseur.Id,
                        Nom = s.Fournisseur.Nom
                    },
                    SousGarantie = s.SousGarantie,
                    Garantie = s.Garantie,
                    AffectationActive = context.Affectations.Any(a => a.StockId == s.Id && a.Actif),
                    ADejaEteAffecte = context.Affectations.Any(a => a.StockId == s.Id),
                    RowVersion = s.RowVersion
                });
        }

        public async Task<Stock> GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Stocks
                .AsNoTracking()
                .Include(s => s.Modele)
                    .ThenInclude(m => m.Marque)
                .Include(s => s.Modele)
                    .ThenInclude(m => m.Materiel)
                        .ThenInclude(m => m.Fiche)
                .Include(s => s.Statut)
                .Include(s => s.Lieu)
                .Include(s => s.Fournisseur)
                .Include(s => s.Systeme)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> HasAffectation(int stockId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Affectations
                .AnyAsync(a => a.StockId == stockId);
        }

        public async Task<bool> HasAffectationHistorique(int stockId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Affectations
                .AnyAsync(a => a.StockId == stockId);
        }

        public async Task<Affectation?> GetAffectationActive(int stockId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Affectations
                .AsNoTracking()
                .Include(a => a.Utilisateur)
                .Include(a => a.Eds)
                .Where(a => a.StockId == stockId && a.Actif)
                .OrderByDescending(a => a.DateDebut)
                .FirstOrDefaultAsync();
        }

        public async Task Add(Stock stock, string effectuePar)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                await ValiderUniciteNumSerieParMarque(context, stock);

                stock.DateMouvement = DateTime.Now;
                context.Stocks.Add(stock);
                await context.SaveChangesAsync();

                // Historique
                var historique = new HistoriqueMouvement
                {
                    StockId = stock.Id,
                    TypeMouvement = "Réception",
                    NouveauStatutId = stock.StatutId,
                    NouveauLieuId = stock.LieuId,
                    DateMouvement = DateTime.Now,
                    EffectuePar = effectuePar,
                    Commentaire = "Réception du matériel"
                };
                context.HistoriqueMouvements.Add(historique);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un matériel avec cet asset existe déjà.", ex);
            }
        }

        public async Task Update(Stock stock, string effectuePar)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                // Récupérer l'ancien état pour l'historique
                var ancien = await context.Stocks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == stock.Id);

                var tracked = await context.Stocks
                    .FirstOrDefaultAsync(s => s.Id == stock.Id);

                if (tracked == null)
                    throw new DbUpdateConcurrencyException();

                context.Entry(tracked)
                    .Property(s => s.RowVersion)
                    .OriginalValue = stock.RowVersion;

                await ValiderUniciteNumSerieParMarque(context, stock);

                tracked.Asset = stock.Asset;
                tracked.Date = stock.Date;
                tracked.DateMouvement = DateTime.Now;
                tracked.NumReception = stock.NumReception;
                tracked.StatutId = stock.StatutId;
                tracked.LieuId = stock.LieuId;
                tracked.Colis = stock.Colis;
                tracked.ModeleId = stock.ModeleId;
                tracked.FournisseurId = stock.FournisseurId;
                tracked.NumSerie = stock.NumSerie;
                tracked.Qte = stock.Qte;
                tracked.SousGarantie = stock.SousGarantie;
                tracked.Garantie = stock.Garantie;
                tracked.SystemeId = stock.SystemeId;
                tracked.NumSim = stock.NumSim;
                tracked.Imei1 = stock.Imei1;
                tracked.Imei2 = stock.Imei2;

                await context.SaveChangesAsync();

                // Historique si statut ou lieu a changé
                if (ancien?.StatutId != stock.StatutId || ancien?.LieuId != stock.LieuId)
                {
                    var historique = new HistoriqueMouvement
                    {
                        StockId = stock.Id,
                        TypeMouvement = "Modification",
                        AncienStatutId = ancien?.StatutId,
                        AncienLieuId = ancien?.LieuId,
                        NouveauStatutId = stock.StatutId,
                        NouveauLieuId = stock.LieuId,
                        DateMouvement = DateTime.Now,
                        EffectuePar = effectuePar
                    };
                    context.HistoriqueMouvements.Add(historique);
                    await context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce matériel a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un matériel avec cet asset existe déjà.", ex);
            }
        }

        private static async Task ValiderUniciteNumSerieParMarque(AppDbContext context, Stock stock)
        {
            if (string.IsNullOrWhiteSpace(stock.NumSerie))
                return;

            var marqueId = await context.Modeles
                .AsNoTracking()
                .Where(m => m.Id == stock.ModeleId)
                .Select(m => m.MarqueId)
                .FirstOrDefaultAsync();

            var numeroSerie = stock.NumSerie.Trim().ToLower();
            var existe = await context.Stocks
                .AsNoTracking()
                .AnyAsync(s =>
                    s.Id != stock.Id &&
                    s.NumSerie.ToLower() == numeroSerie &&
                    s.Modele.MarqueId == marqueId);

            if (existe)
                throw new InvalidOperationException("Un matériel de cette marque avec ce numéro de série existe déjà.");
        }

        public async Task Delete(Stock stock)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                // Vérifier pas d'affectation
                var aUneAffectation = await context.Affectations
                    .AnyAsync(a => a.StockId == stock.Id);

                if (aUneAffectation)
                    throw new InvalidOperationException("Impossible de supprimer : ce matériel a des affectations.");

                var historiques = await context.HistoriqueMouvements
                    .Where(h => h.StockId == stock.Id)
                    .ToListAsync();

                context.HistoriqueMouvements.RemoveRange(historiques);

                var tracked = new Stock { Id = stock.Id, RowVersion = stock.RowVersion };
                context.Stocks.Attach(tracked);
                context.Stocks.Remove(tracked);
                await context.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce matériel a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : ce matériel est utilisé.", ex);
            }
        }
    }
}
