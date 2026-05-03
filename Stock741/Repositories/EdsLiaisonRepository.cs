using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class EdsLiaisonRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public EdsLiaisonRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        //public async Task<List<EdsLiaison>> GetAll()
        //{
        //    using var context = _contextFactory.CreateDbContext();
        //    return await context.EdsLiaisons
        //        .AsNoTracking()
        //        .Include(el => el.Eds)
        //        .OrderBy(el => el.Cible)
        //        .ToListAsync();
        //}

        public async Task<List<EdsLiaison>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.EdsLiaisons
                .AsNoTracking()
                .OrderBy(el => el.Cible)
                .Select(el => new EdsLiaison
                {
                    Id = el.Id,
                    Cible = el.Cible,
                    EdsId = el.EdsId,
                    RowVersion = el.RowVersion,
                    Eds = new Eds
                    {
                        Id = el.Eds.Id,
                        Cnx = el.Eds.Cnx,
                        Nom = el.Eds.Nom
                    }
                })
                .ToListAsync();
        }

        public async Task Add(EdsLiaison edsLiaison)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.EdsLiaisons.Add(edsLiaison);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Une liaison avec ce code existe déjà.", ex);
            }
        }

        public async Task Update(EdsLiaison edsLiaison)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.EdsLiaisons.Update(edsLiaison);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Cette liaison a été modifiée ou supprimée par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Une liaison avec ce code existe déjà.", ex);
            }
        }

        public async Task Delete(EdsLiaison edsLiaison)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tracked = new EdsLiaison { Id = edsLiaison.Id, RowVersion = edsLiaison.RowVersion };
                context.EdsLiaisons.Attach(tracked);
                context.EdsLiaisons.Remove(tracked);
                await context.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Cette liaison a été modifiée ou supprimée par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : cette liaison est utilisée.", ex);
            }
        }
    }
}
