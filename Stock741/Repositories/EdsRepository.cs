using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class EdsRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public EdsRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Eds>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Eds
                .AsNoTracking()
                .OrderBy(e => e.Nom)
                .ToListAsync();
        }

        public async Task Add(Eds eds)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Eds.Add(eds);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un EDS avec ce code existe déjà.", ex);
            }
        }

        public async Task Update(Eds eds)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                context.Eds.Update(eds);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Cet EDS a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                throw new InvalidOperationException("Un EDS avec ce code existe déjà.", ex);
            }
        }

        public async Task Delete(Eds eds)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var tracked = new Eds { Id = eds.Id, RowVersion = eds.RowVersion };
                context.Eds.Attach(tracked);
                context.Eds.Remove(tracked);
                await context.SaveChangesAsync();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Cet EDS a été modifié ou supprimé par un autre utilisateur. Veuillez actualiser la vue.");
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : cet EDS est utilisé.", ex);
            }
        }
    }
}


//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using Stock741.Data;
//using Stock741.Models;

//namespace Stock741.Repositories
//{
//    public class EdsRepository
//    {
//        private readonly IDbContextFactory<AppDbContext> _contextFactory;

//        public EdsRepository(IDbContextFactory<AppDbContext> contextFactory)
//        {
//            _contextFactory = contextFactory;
//        }

//        public async Task<List<Eds>> GetAll()
//        {
//            using var context = _contextFactory.CreateDbContext();
//            return await context.Eds
//                .AsNoTracking()
//                .OrderBy(e => e.Nom)
//                .ToListAsync();
//        }

//        public async Task Add(Eds eds)
//        {
//            try
//            {
//                using var context = _contextFactory.CreateDbContext();
//                context.Eds.Add(eds);
//                await context.SaveChangesAsync();
//            }
//            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
//                                                (ex.InnerException as SqlException)?.Number == 2627)
//            {
//                throw new InvalidOperationException("Un EDS avec ce code existe déjà.", ex);
//            }
//        }

//        public async Task Update(Eds eds)
//        {
//            try
//            {
//                using var context = _contextFactory.CreateDbContext();
//                context.Eds.Update(eds);
//                await context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                throw new InvalidOperationException("Cet EDS a été modifié ou supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
//            }
//            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
//                                                (ex.InnerException as SqlException)?.Number == 2627)
//            {
//                throw new InvalidOperationException("Un EDS avec ce code existe déjà.", ex);
//            }
//        }

//        public async Task Delete(Eds eds)
//        {
//            try
//            {
//                using var context = _contextFactory.CreateDbContext();
//                var tracked = await context.Eds.FindAsync(eds.Id);
//                if (tracked == null)
//                    throw new InvalidOperationException("Cet EDS a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
//                context.Eds.Remove(tracked);
//                await context.SaveChangesAsync();
//            }
//            catch (InvalidOperationException)
//            {
//                throw;
//            }
//            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
//            {
//                throw new InvalidOperationException("Impossible de supprimer : cet EDS est utilisé.", ex);
//            }
//        }
//    }
//}