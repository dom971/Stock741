using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class OperateurRepository
    {
        private readonly AppDbContext _context;

        public OperateurRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Operateur> GetAll()
        {
            return _context.Operateurs
                .AsNoTracking()
                .OrderBy(o => o.Nom)
                .ToList();
        }

        public void Add(Operateur operateur)
        {
            _context.Operateurs.Add(operateur);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(operateur).State = EntityState.Detached;
                throw new InvalidOperationException("Un opérateur avec ce nom existe déjà.", ex);
            }
        }

        public void Update(Operateur operateur)
        {
            _context.Operateurs.Update(operateur);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(operateur).State = EntityState.Detached;
                throw new InvalidOperationException("Un opérateur avec ce nom existe déjà.", ex);
            }
        }

        public void Delete(Operateur operateur)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();

                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "DELETE FROM Operateurs WHERE Id = @Id";
                    var param = commande.CreateParameter();
                    param.ParameterName = "@Id";
                    param.Value = operateur.Id;
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
                throw new InvalidOperationException("Impossible de supprimer : cet opérateur est utilisé.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
            }
        }
    }
}
