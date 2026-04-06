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
            return _context.Marques
                .AsNoTracking()
                .OrderBy(m => m.Nom)
                .ToList();
        }

        public void Add(Marque marque)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "INSERT INTO Marques (Nom, Actif) VALUES (@Nom, @Actif); SELECT SCOPE_IDENTITY();";
                    var p1 = commande.CreateParameter();
                    p1.ParameterName = "@Nom";
                    p1.Value = marque.Nom;
                    commande.Parameters.Add(p1);
                    var p2 = commande.CreateParameter();
                    p2.ParameterName = "@Actif";
                    p2.Value = marque.Actif;
                    commande.Parameters.Add(p2);
                    var id = commande.ExecuteScalar();
                    marque.Id = Convert.ToInt32(id);
                }
                finally
                {
                    connexion.Close();
                }
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                throw new InvalidOperationException("Une marque avec ce nom existe déjà.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de l'ajout.", ex);
            }
        }

        public void Update(Marque marque)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "UPDATE Marques SET Nom = @Nom, Actif = @Actif WHERE Id = @Id";
                    var p1 = commande.CreateParameter();
                    p1.ParameterName = "@Nom";
                    p1.Value = marque.Nom;
                    commande.Parameters.Add(p1);
                    var p2 = commande.CreateParameter();
                    p2.ParameterName = "@Actif";
                    p2.Value = marque.Actif;
                    commande.Parameters.Add(p2);
                    var p3 = commande.CreateParameter();
                    p3.ParameterName = "@Id";
                    p3.Value = marque.Id;
                    commande.Parameters.Add(p3);
                    var lignesAffectees = commande.ExecuteNonQuery();
                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Cette marque a été supprimée par un autre utilisateur. Veuillez rafraîchir la vue.");
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
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                throw new InvalidOperationException("Une marque avec ce nom existe déjà.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la modification.", ex);
            }
        }

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
                    var lignesAffectees = commande.ExecuteNonQuery();
                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Cette marque a été supprimée par un autre utilisateur. Veuillez rafraîchir la vue.");
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
                throw new InvalidOperationException("Impossible de supprimer : cette marque est utilisée dans un modèle.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
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
//    public class MarqueRepository
//    {
//        private readonly AppDbContext _context;

//        public MarqueRepository(AppDbContext context)
//        {
//            _context = context;
//        }

//        //public List<Marque> GetAll()
//        //{
//        //    return _context.Marques.OrderBy(m => m.Nom).ToList();
//        //}

//        public List<Marque> GetAll()
//        {
//            return _context.Marques
//                .AsNoTracking()
//                .OrderBy(m => m.Nom)
//                .ToList();

//            //return _context.Marques
//            //    .OrderBy(m => m.Nom)
//            //    .ToList();


//        }

//        public void Add(Marque marque)
//        {
//            _context.Marques.Add(marque);
//            try
//            {
//                _context.SaveChanges();
//            }
//            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
//                                                (ex.InnerException as SqlException)?.Number == 2627)
//            {
//                _context.Entry(marque).State = EntityState.Detached;
//                throw new InvalidOperationException("Une marque avec ce nom existe déjà.", ex);
//            }
//        }

//        public void Update(Marque marque)
//        {
//            _context.Marques.Update(marque);
//            try
//            {
//                _context.SaveChanges();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                _context.Entry(marque).State = EntityState.Detached;
//                throw new InvalidOperationException("Cette marque a été modifiée ou supprimée par un autre utilisateur. Veuillez rafraîchir la vue.", null);
//            }
//            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
//                                                (ex.InnerException as SqlException)?.Number == 2627)
//            {
//                _context.Entry(marque).State = EntityState.Detached;
//                throw new InvalidOperationException("Une marque avec ce nom existe déjà.", ex);
//            }
//        }

//        public void Delete(Marque marque)
//        {
//            try
//            {
//                var connexion = _context.Database.GetDbConnection();
//                connexion.Open();

//                try
//                {
//                    using var commande = connexion.CreateCommand();
//                    commande.CommandText = "DELETE FROM Marques WHERE Id = @Id";
//                    var param = commande.CreateParameter();
//                    param.ParameterName = "@Id";
//                    param.Value = marque.Id;
//                    commande.Parameters.Add(param);
//                    var lignesAffectees = commande.ExecuteNonQuery();

//                    if (lignesAffectees == 0)
//                        throw new InvalidOperationException("Cette marque a été supprimée par un autre utilisateur. Veuillez rafraîchir la vue.");
//                }
//                finally
//                {
//                    connexion.Close();
//                }
//            }
//            catch (InvalidOperationException)
//            {
//                throw;
//            }
//            catch (SqlException ex) when (ex.Number == 547)
//            {
//                throw new InvalidOperationException("Impossible de supprimer : cette marque est utilisée dans un modèle.", ex);
//            }
//            catch (SqlException ex)
//            {
//                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
//            }
//        }



//    }
//}