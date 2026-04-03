using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class MarqueRepository
    {
        private readonly AppDbContext _context;

        public MarqueRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Marque> GetAll()
        {
            return _context.Marques.OrderBy(m => m.Nom).ToList();
        }

        public void Add(Marque marque)
        {
            _context.Marques.Add(marque);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(marque).State = EntityState.Detached;
                throw new InvalidOperationException("Une marque avec ce nom existe déjà.", ex);
            }
        }

        public void Update(Marque marque)
        {
            _context.Marques.Update(marque);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(marque).State = EntityState.Detached;
                throw new InvalidOperationException("Une marque avec ce nom existe déjà.", ex);
            }
        }

        //public void Delete(Marque marque)
        //{
        //    var tracked = _context.Marques.Find(marque.Id);
        //    if (tracked == null)
        //        throw new InvalidOperationException("Marque introuvable.");

        //    _context.Marques.Remove(tracked);
        //    try
        //    {
        //        _context.SaveChanges();
        //    }
        //    catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 547)
        //    {
        //        _context.Entry(tracked).State = EntityState.Unchanged; // ← restaure l'état
        //        throw new InvalidOperationException("Impossible de supprimer : cette marque est utilisée dans un modèle.", ex);
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        _context.Entry(tracked).State = EntityState.Unchanged; // ← restaure l'état
        //        throw new InvalidOperationException("Erreur lors de la suppression.", ex);
        //    }
        //}

        public void Delete(Marque marque)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();

                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "DELETE FROM Marques WHERE Id = @Id";
                    var param = commande.CreateParameter();
                    param.ParameterName = "@Id";
                    param.Value = marque.Id;
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
                throw new InvalidOperationException("Impossible de supprimer : cette marque est utilisée dans un modèle.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
            }
        }

    }
}