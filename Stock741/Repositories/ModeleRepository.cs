using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class ModeleRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ModeleRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Modele>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Modeles
                .AsNoTracking()
                .Include(m => m.Marque)
                .Include(m => m.Materiel)
                    .ThenInclude(m => m.Fiche)
                .OrderBy(m => m.Nom)
                .ToListAsync();
        }

        public async Task Add(Modele modele)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Modeles.Add(modele);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un modèle avec ce nom existe déjà.", ex);
            }
        }

        public async Task Update(Modele modele)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Modeles.Update(modele);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce modèle a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un modèle avec ce nom existe déjà.", ex);
            }
        }

        public async Task Delete(Modele modele)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tracked = new Modele { Id = modele.Id, RowVersion = modele.RowVersion };
                context.Modeles.Attach(tracked);
                context.Modeles.Remove(tracked);
                await context.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce modèle a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : ce modèle est utilisé.", ex);
            }
        }
    }
}