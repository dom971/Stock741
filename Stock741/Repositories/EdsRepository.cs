using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class EdsRepository
    {
        private readonly AppDbContext _context;

        public EdsRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Eds> GetAll()
        {
            return _context.Eds
                .AsNoTracking().
                 OrderBy(e => e.Nom)               
                .ToList();
        }

        public void Add(Eds eds)
        {
            _context.Eds.Add(eds);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(eds).State = EntityState.Detached;
                throw new InvalidOperationException("Un EDS avec ce code existe déjà.", ex);
            }
        }

        public void Update(Eds eds)
        {
            _context.Eds.Update(eds);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                _context.Entry(eds).State = EntityState.Detached;
                throw new InvalidOperationException("Cet EDS a été modifié ou supprimé par un autre utilisateur. Veuillez rafraîchir la vue.", null);
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(eds).State = EntityState.Detached;
                throw new InvalidOperationException("Un EDS avec ce code existe déjà.", ex);
            }
        }

        public void Delete(Eds eds)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();

                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "DELETE FROM Eds WHERE Id = @Id";
                    var param = commande.CreateParameter();
                    param.ParameterName = "@Id";
                    param.Value = eds.Id;
                    commande.Parameters.Add(param);
                    var lignesAffectees = commande.ExecuteNonQuery();

                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Cet EDS a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
                }
                finally
                {
                    connexion.Close();
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : cet EDS est utilisé.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
            }
        }

        //public void Update(Eds eds)
        //{
        //    _context.Eds.Update(eds);
        //    try
        //    {
        //        _context.SaveChanges();
        //    }
        //    catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
        //                                        (ex.InnerException as SqlException)?.Number == 2627)
        //    {
        //        _context.Entry(eds).State = EntityState.Detached;
        //        throw new InvalidOperationException("Un EDS avec ce code existe déjà.", ex);
        //    }
        //}

        //public void Delete(Eds eds)
        //{
        //    try
        //    {
        //        var connexion = _context.Database.GetDbConnection();
        //        connexion.Open();

        //        try
        //        {
        //            using var commande = connexion.CreateCommand();
        //            commande.CommandText = "DELETE FROM Eds WHERE Id = @Id";
        //            var param = commande.CreateParameter();
        //            param.ParameterName = "@Id";
        //            param.Value = eds.Id;
        //            commande.Parameters.Add(param);
        //            commande.ExecuteNonQuery();
        //        }
        //        finally
        //        {
        //            connexion.Close();
        //        }
        //    }
        //    catch (SqlException ex) when (ex.Number == 547)
        //    {
        //        throw new InvalidOperationException("Impossible de supprimer : cet EDS est utilisé.", ex);
        //    }
        //    catch (SqlException ex)
        //    {
        //        throw new InvalidOperationException("Erreur lors de la suppression.", ex);
        //    }
        //}
    }
}
