using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class MaterielRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public MaterielRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Materiel>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Materiels
                .AsNoTracking()
                .Include(m => m.Fiche)
                .OrderBy(m => m.Nom)
                .ToListAsync();
        }

        public async Task Add(Materiel materiel)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Materiels.Add(materiel);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un matériel avec ce nom existe déjà.", ex);
            }
        }

        public async Task Update(Materiel materiel)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Materiels.Update(materiel);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Ce matériel a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un matériel avec ce nom existe déjà.", ex);
            }
        }

        public async Task Delete(Materiel materiel)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tracked = new Materiel { Id = materiel.Id, RowVersion = materiel.RowVersion };
                context.Materiels.Attach(tracked);
                context.Materiels.Remove(tracked);
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
                throw new InvalidOperationException("Impossible de supprimer : ce matériel est utilisé dans un modèle.", ex);
            }
        }
    }
}