using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class SystemeRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public SystemeRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Systeme>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Systemes
                .AsNoTracking()
                .OrderBy(s => s.Nom)
                .ToListAsync();
        }

        public async Task Add(Systeme systeme)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Systemes.Add(systeme);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un système avec ce nom existe déjà.", ex);
            }
        }

        public async Task Update(Systeme systeme)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Systemes.Update(systeme);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce système a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un système avec ce nom existe déjà.", ex);
            }
        }

        public async Task Delete(Systeme systeme)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tracked = new Systeme { Id = systeme.Id, RowVersion = systeme.RowVersion };
                context.Systemes.Attach(tracked);
                context.Systemes.Remove(tracked);
                await context.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce système a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : ce système est utilisé.", ex);
            }
        }
    }
}