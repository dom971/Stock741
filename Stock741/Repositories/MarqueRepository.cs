using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class MarqueRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public MarqueRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Marque>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Marques
                .AsNoTracking()
                .OrderBy(m => m.Nom)
                .ToListAsync();
        }

        public async Task Add(Marque marque)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Marques.Add(marque);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Une marque avec ce nom existe déjà.", ex);
            }
        }

        public async Task Update(Marque marque)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Marques.Update(marque);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Cette marque a été modifiée ou supprimée par un autre utilisateur. Veuillez rafraîchir la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Une marque avec ce nom existe déjà.", ex);
            }
        }

        public async Task Delete(Marque marque)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tracked = new Marque { Id = marque.Id, RowVersion = marque.RowVersion };
                context.Marques.Attach(tracked);
                context.Marques.Remove(tracked);
                await context.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Cette marque a été modifiée ou supprimée par un autre utilisateur. Veuillez rafraîchir la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : cette marque est utilisée dans un modèle.", ex);
            }
        }
    }
}