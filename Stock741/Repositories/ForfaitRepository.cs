using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class ForfaitRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ForfaitRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Forfait>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Forfaits
                .AsNoTracking()
                .Include(f => f.Operateur)
                .OrderBy(f => f.Operateur.Nom)
                .ThenBy(f => f.Nom)
                .ToListAsync();
        }

        public async Task Add(Forfait forfait)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Forfaits.Add(forfait);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un forfait avec ce nom existe déjà.", ex);
            }
        }

        public async Task Update(Forfait forfait)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Forfaits.Update(forfait);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce forfait a été modifié ou supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un forfait avec ce nom existe déjà.", ex);
            }
        }

        public async Task Delete(Forfait forfait)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tracked = await context.Forfaits.FindAsync(forfait.Id);
                if (tracked == null)
                    throw new InvalidOperationException("Ce forfait a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
                context.Forfaits.Remove(tracked);
                await context.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : ce forfait est utilisé.", ex);
            }
        }
    }
}