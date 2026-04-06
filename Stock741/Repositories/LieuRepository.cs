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
            return _context.Lieux
                .AsNoTracking()
                .OrderBy(l => l.Nom)
                .ToList();
        }

        public void Add(Lieu lieu)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = @"INSERT INTO Lieux (Nom) 
                                            VALUES (@Nom); 
                                            SELECT SCOPE_IDENTITY();";
                    var p1 = commande.CreateParameter();
                    p1.ParameterName = "@Nom";
                    p1.Value = lieu.Nom;
                    commande.Parameters.Add(p1);
                    var id = commande.ExecuteScalar();
                    lieu.Id = Convert.ToInt32(id);
                }
                finally
                {
                    connexion.Close();
                }
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                throw new InvalidOperationException("Un lieu avec ce nom existe déjà.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de l'ajout.", ex);
            }
        }

        public void Update(Lieu lieu)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "UPDATE Lieux SET Nom = @Nom WHERE Id = @Id";
                    var p1 = commande.CreateParameter();
                    p1.ParameterName = "@Nom";
                    p1.Value = lieu.Nom;
                    commande.Parameters.Add(p1);
                    var p2 = commande.CreateParameter();
                    p2.ParameterName = "@Id";
                    p2.Value = lieu.Id;
                    commande.Parameters.Add(p2);
                    var lignesAffectees = commande.ExecuteNonQuery();
                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Ce lieu a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
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
                throw new InvalidOperationException("Un lieu avec ce nom existe déjà.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la modification.", ex);
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
                    var lignesAffectees = commande.ExecuteNonQuery();
                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Ce lieu a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
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
                throw new InvalidOperationException("Impossible de supprimer : ce lieu est utilisé.", ex);
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
//    public class LieuRepository
//    {
//        private readonly AppDbContext _context;

//        public LieuRepository(AppDbContext context)
//        {
//            _context = context;
//        }

//        public List<Lieu> GetAll()
//        {
//            return _context.Lieux
//                .AsNoTracking()
//                .OrderBy(l => l.Nom)
//                .ToList();
//        }

//        public void Add(Lieu lieu)
//        {
//            _context.Lieux.Add(lieu);
//            try
//            {
//                _context.SaveChanges();
//            }
//            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
//                                                (ex.InnerException as SqlException)?.Number == 2627)
//            {
//                _context.Entry(lieu).State = EntityState.Detached;
//                throw new InvalidOperationException("Un lieu avec ce nom existe déjà.", ex);
//            }
//        }

//        public void Update(Lieu lieu)
//        {
//            _context.Lieux.Update(lieu);
//            try
//            {
//                _context.SaveChanges();
//            }
//            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
//                                                (ex.InnerException as SqlException)?.Number == 2627)
//            {
//                _context.Entry(lieu).State = EntityState.Detached;
//                throw new InvalidOperationException("Un lieu avec ce nom existe déjà.", ex);
//            }
//        }

//        public void Delete(Lieu lieu)
//        {
//            try
//            {
//                var connexion = _context.Database.GetDbConnection();
//                connexion.Open();

//                try
//                {
//                    using var commande = connexion.CreateCommand();
//                    commande.CommandText = "DELETE FROM Lieux WHERE Id = @Id";
//                    var param = commande.CreateParameter();
//                    param.ParameterName = "@Id";
//                    param.Value = lieu.Id;
//                    commande.Parameters.Add(param);
//                    commande.ExecuteNonQuery();
//                }
//                finally
//                {
//                    connexion.Close();
//                }
//            }
//            catch (SqlException ex) when (ex.Number == 547)
//            {
//                throw new InvalidOperationException("Impossible de supprimer : ce lieu est utilisé.", ex);
//            }
//            catch (SqlException ex)
//            {
//                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
//            }
//        }
//    }
//}
