using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class LieuRepository
    {
        private readonly AppDbContext _context;

        public LieuRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Lieu> GetAll()
        {
            return _context.Lieux.OrderBy(l => l.Nom).ToList();
        }

        public void Add(Lieu lieu)
        {
            _context.Lieux.Add(lieu);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(lieu).State = EntityState.Detached;
                throw new InvalidOperationException("Un lieu avec ce nom existe déjà.", ex);
            }
        }

        public void Update(Lieu lieu)
        {
            _context.Lieux.Update(lieu);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(lieu).State = EntityState.Detached;
                throw new InvalidOperationException("Un lieu avec ce nom existe déjà.", ex);
            }
        }

        public void Delete(Lieu lieu)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();

                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "DELETE FROM Lieux WHERE Id = @Id";
                    var param = commande.CreateParameter();
                    param.ParameterName = "@Id";
                    param.Value = lieu.Id;
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
                throw new InvalidOperationException("Impossible de supprimer : ce lieu est utilisé.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
            }
        }
    }
}
