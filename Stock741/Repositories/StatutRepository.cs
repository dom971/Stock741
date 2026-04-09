using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class StatutRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public StatutRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Statut>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Statuts
                .AsNoTracking()
                .OrderBy(s => s.Nom)
                .ToListAsync();
        }

        public async Task Add(Statut statut)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Statuts.Add(statut);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un statut avec ce nom existe déjà.", ex);
            }
        }

        public async Task Update(Statut statut)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Statuts.Update(statut);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce statut a été modifié ou supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un statut avec ce nom existe déjà.", ex);
            }
        }

        public async Task Delete(Statut statut)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tracked = await context.Statuts.FindAsync(statut.Id);
                if (tracked == null)
                    throw new InvalidOperationException("Ce statut a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
                context.Statuts.Remove(tracked);
                await context.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : ce statut est utilisé.", ex);
            }
        }
    }
}