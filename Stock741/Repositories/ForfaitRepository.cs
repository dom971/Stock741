using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class ForfaitRepository
    {
        private readonly AppDbContext _context;

        public ForfaitRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Forfait> GetAll()
        {
            return _context.Forfaits
        .Include(f => f.Operateur)
        .OrderBy(f => f.Operateur.Nom)
        .ThenBy(f => f.Nom)
        .ToList();
        }

        public void Add(Forfait forfait)
        {
            _context.Forfaits.Add(forfait);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(forfait).State = EntityState.Detached;
                throw new InvalidOperationException("Un forfait avec ce nom existe déjà.", ex);
            }
        }

        public void Update(Forfait forfait)
        {
            _context.Forfaits.Update(forfait);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(forfait).State = EntityState.Detached;
                throw new InvalidOperationException("Un forfait avec ce nom existe déjà.", ex);
            }
        }

        public void Delete(Forfait forfait)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();

                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "DELETE FROM Forfaits WHERE Id = @Id";
                    var param = commande.CreateParameter();
                    param.ParameterName = "@Id";
                    param.Value = forfait.Id;
                    commande.Parameters.Add(param);
                    commande.ExecuteNonQuery();
                }
                finally
                {
                    connexion.Close();
                }
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new InvalidOperationException("Impossible de supprimer : ce forfait est utilisé.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
            }
        }
    }
}