using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class FicheRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public FicheRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Fiche>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Fiches
                .AsNoTracking()
                .OrderBy(f => f.Nom)
                .ToListAsync();
        }

        public async Task Add(Fiche fiche)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Fiches.Add(fiche);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Une fiche avec ce nom existe déjà.", ex);
            }
        }

        public async Task Update(Fiche fiche)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Fiches.Update(fiche);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Cette fiche a été modifiée ou supprimée par un autre utilisateur. Veuillez rafraîchir la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Une fiche avec ce nom existe déjà.", ex);
            }
        }

        public async Task Delete(Fiche fiche)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tracked = await context.Fiches.FindAsync(fiche.Id);
                if (tracked == null)
                    throw new InvalidOperationException("Cette fiche a été supprimée par un autre utilisateur. Veuillez rafraîchir la vue.");
                context.Fiches.Remove(tracked);
                await context.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : cette fiche est utilisée.", ex);
            }
        }
    }
}