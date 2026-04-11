using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class FournisseurRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public FournisseurRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Fournisseur>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Fournisseurs
                .AsNoTracking()
                .OrderBy(f => f.Nom)
                .ToListAsync();
        }

        public async Task Add(Fournisseur fournisseur)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Fournisseurs.Add(fournisseur);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un fournisseur avec ce nom existe déjà.", ex);
            }
        }

        public async Task Update(Fournisseur fournisseur)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Fournisseurs.Update(fournisseur);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce fournisseur a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un fournisseur avec ce nom existe déjà.", ex);
            }
        }

        public async Task Delete(Fournisseur fournisseur)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tracked = new Fournisseur { Id = fournisseur.Id, RowVersion = fournisseur.RowVersion };
                context.Fournisseurs.Attach(tracked);
                context.Fournisseurs.Remove(tracked);
                await context.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce fournisseur a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : ce fournisseur est utilisé.", ex);
            }
        }
    }
}