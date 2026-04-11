using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class LieuRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public LieuRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Lieu>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Lieux
                .AsNoTracking()
                .OrderBy(l => l.Nom)
                .ToListAsync();
        }

        public async Task Add(Lieu lieu)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Lieux.Add(lieu);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un lieu avec ce nom existe déjà.", ex);
            }
        }

        public async Task Update(Lieu lieu)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Lieux.Update(lieu);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce lieu a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un lieu avec ce nom existe déjà.", ex);
            }
        }

        public async Task Delete(Lieu lieu)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tracked = new Lieu { Id = lieu.Id, RowVersion = lieu.RowVersion };
                context.Lieux.Attach(tracked);
                context.Lieux.Remove(tracked);
                await context.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce lieu a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : ce lieu est utilisé.", ex);
            }
        }
    }
}