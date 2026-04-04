using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class FicheRepository
    {
        private readonly AppDbContext _context;

        public FicheRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Fiche> GetAll()
        {
            return _context.Fiches.OrderBy(f => f.Nom).ToList();
        }

        public void Add(Fiche fiche)
        {
            _context.Fiches.Add(fiche);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(fiche).State = EntityState.Detached;
                throw new InvalidOperationException("Une fiche avec ce nom existe déjà.", ex);
            }
        }

        public void Update(Fiche fiche)
        {
            _context.Fiches.Update(fiche);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(fiche).State = EntityState.Detached;
                throw new InvalidOperationException("Une fiche avec ce nom existe déjà.", ex);
            }
        }

        public void Delete(Fiche fiche)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();

                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "DELETE FROM Fiches WHERE Id = @Id";
                    var param = commande.CreateParameter();
                    param.ParameterName = "@Id";
                    param.Value = fiche.Id;
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
                throw new InvalidOperationException("Impossible de supprimer : cette fiche est utilisée.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
            }
        }
    }
}