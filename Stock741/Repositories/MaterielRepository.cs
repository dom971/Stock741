using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;
using System.Windows.Media.Media3D;

namespace Stock741.Repositories
{
    public class MaterielRepository
    {
        private readonly AppDbContext _context;

        public MaterielRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Materiel> GetAll()
        {
            return _context.Materiels.OrderBy(m => m.Nom).ToList();
        }

        public void Add(Materiel materiel)
        {
            _context.Materiels.Add(materiel);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(materiel).State = EntityState.Detached;
                throw new InvalidOperationException("Un matériel avec ce nom existe déjà.", ex);
            }
        }

        public void Update(Materiel materiel)
        {
            _context.Materiels.Update(materiel);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(materiel).State = EntityState.Detached;
                throw new InvalidOperationException("Un matériel avec ce nom existe déjà.", ex);
            }
        }

        public void Delete(Materiel materiel)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();

                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "DELETE FROM Materiels WHERE Id = @Id";
                    var param = commande.CreateParameter();
                    param.ParameterName = "@Id";
                    param.Value = materiel.Id;
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
                throw new InvalidOperationException("Impossible de supprimer : ce matériel est utilisé dans un modèle.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
            }
        }
    }
}
