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
            return await context.Stocks
                .AsNoTracking()
                .Include(s => s.Modele)
                    .ThenInclude(m => m.Materiel)
                        .ThenInclude(m => m.Fiche)
                .Include(s => s.Statut)
                .Include(s => s.Lieu)
                .OrderBy(s => s.Date)
                .Select(s => new Stock
                {
                    Id = s.Id,
                    Asset = s.Asset,
                    NumSerie = s.NumSerie,
                    Date = s.Date,
                    NumReception = s.NumReception,
                    StatutId = s.StatutId,
                    Statut = s.Statut,
                    LieuId = s.LieuId,
                    Lieu = s.Lieu,
                    ModeleId = s.ModeleId,
                    Modele = s.Modele,
                    RowVersion = s.RowVersion
                })
                .ToListAsync();
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

        public async Task Add(Stock stock, string effectuePar)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Stocks.Add(stock);
                await context.SaveChangesAsync();

                // Historique
                var historique = new HistoriqueMouvement
                {
                    StockId = stock.Id,
                    TypeMouvement = "Réception",
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
                throw new InvalidOperationException("Un matériel avec cet asset ou ce numéro de série existe déjà.", ex);
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

                context.Stocks.Update(stock);
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
                throw new InvalidOperationException("Un matériel avec cet asset ou ce numéro de série existe déjà.", ex);
            }
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