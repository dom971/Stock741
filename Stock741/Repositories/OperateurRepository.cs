using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class OperateurRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public OperateurRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Operateur>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Operateurs
                .AsNoTracking()
                .OrderBy(o => o.Nom)
                .ToListAsync();
        }

        public async Task Add(Operateur operateur)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Operateurs.Add(operateur);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un opérateur avec ce nom existe déjà.", ex);
            }
        }

        public async Task Update(Operateur operateur)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Operateurs.Update(operateur);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Cet opérateur a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un opérateur avec ce nom existe déjà.", ex);
            }
        }

        public async Task Delete(Operateur operateur)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tracked = new Operateur { Id = operateur.Id, RowVersion = operateur.RowVersion };
                context.Operateurs.Attach(tracked);
                context.Operateurs.Remove(tracked);
                await context.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Cet opérateur a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : cet opérateur est utilisé.", ex);
            }
        }
    }
}