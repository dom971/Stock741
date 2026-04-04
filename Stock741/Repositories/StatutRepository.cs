using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class StatutRepository
    {
        private readonly AppDbContext _context;

        public StatutRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Statut> GetAll()
        {
            return _context.Statuts.OrderBy(s => s.Nom).ToList();
        }

        public void Add(Statut statut)
        {
            _context.Statuts.Add(statut);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(statut).State = EntityState.Detached;
                throw new InvalidOperationException("Un statut avec ce nom existe déjà.", ex);
            }
        }

        public void Update(Statut statut)
        {
            _context.Statuts.Update(statut);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(statut).State = EntityState.Detached;
                throw new InvalidOperationException("Un statut avec ce nom existe déjà.", ex);
            }
        }

        public void Delete(Statut statut)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();

                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "DELETE FROM Statuts WHERE Id = @Id";
                    var param = commande.CreateParameter();
                    param.ParameterName = "@Id";
                    param.Value = statut.Id;
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
                throw new InvalidOperationException("Impossible de supprimer : ce statut est utilisé.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
            }
        }
    }
}