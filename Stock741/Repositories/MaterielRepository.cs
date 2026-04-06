using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

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
            return _context.Materiels
                .AsNoTracking()
                .Include(m => m.Fiche)
                .OrderBy(m => m.Nom)
                .ToList();
        }

        public void Add(Materiel materiel)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = @"INSERT INTO Materiels (Nom, Actif, FicheId) 
                                            VALUES (@Nom, @Actif, @FicheId); 
                                            SELECT SCOPE_IDENTITY();";
                    var p1 = commande.CreateParameter();
                    p1.ParameterName = "@Nom";
                    p1.Value = materiel.Nom;
                    commande.Parameters.Add(p1);
                    var p2 = commande.CreateParameter();
                    p2.ParameterName = "@Actif";
                    p2.Value = materiel.Actif;
                    commande.Parameters.Add(p2);
                    var p3 = commande.CreateParameter();
                    p3.ParameterName = "@FicheId";
                    p3.Value = materiel.FicheId;
                    commande.Parameters.Add(p3);
                    var id = commande.ExecuteScalar();
                    materiel.Id = Convert.ToInt32(id);
                }
                finally
                {
                    connexion.Close();
                }
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                throw new InvalidOperationException("Un matériel avec ce nom existe déjà.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de l'ajout.", ex);
            }
        }

        public void Update(Materiel materiel)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = @"UPDATE Materiels 
                                            SET Nom = @Nom, Actif = @Actif, FicheId = @FicheId 
                                            WHERE Id = @Id";
                    var p1 = commande.CreateParameter();
                    p1.ParameterName = "@Nom";
                    p1.Value = materiel.Nom;
                    commande.Parameters.Add(p1);
                    var p2 = commande.CreateParameter();
                    p2.ParameterName = "@Actif";
                    p2.Value = materiel.Actif;
                    commande.Parameters.Add(p2);
                    var p3 = commande.CreateParameter();
                    p3.ParameterName = "@FicheId";
                    p3.Value = materiel.FicheId;
                    commande.Parameters.Add(p3);
                    var p4 = commande.CreateParameter();
                    p4.ParameterName = "@Id";
                    p4.Value = materiel.Id;
                    commande.Parameters.Add(p4);
                    var lignesAffectees = commande.ExecuteNonQuery();
                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Ce matériel a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
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
                throw new InvalidOperationException("Un matériel avec ce nom existe déjà.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la modification.", ex);
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
                    var lignesAffectees = commande.ExecuteNonQuery();
                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Ce matériel a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
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
                throw new InvalidOperationException("Impossible de supprimer : ce matériel est utilisé dans un modèle.", ex);
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
//using System.Windows.Media.Media3D;

//namespace Stock741.Repositories
//{
//    public class MaterielRepository
//    {
//        private readonly AppDbContext _context;

//        public MaterielRepository(AppDbContext context)
//        {
//            _context = context;
//        }

//        public List<Materiel> GetAll()
//        {
//            return _context.Materiels
//              .AsNoTracking()  
//              .Include(m => m.Fiche)
//              .OrderBy(m => m.Nom)
//              .ToList();
//        }

//        public void Add(Materiel materiel)
//        {
//            _context.Materiels.Add(materiel);
//            try
//            {
//                _context.SaveChanges();
//            }
//            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
//                                                (ex.InnerException as SqlException)?.Number == 2627)
//            {
//                _context.Entry(materiel).State = EntityState.Detached;
//                throw new InvalidOperationException("Un matériel avec ce nom existe déjà.", ex);
//            }
//        }

//        public void Update(Materiel materiel)
//        {
//            _context.Materiels.Update(materiel);
//            try
//            {
//                _context.SaveChanges();
//            }
//            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
//                                                (ex.InnerException as SqlException)?.Number == 2627)
//            {
//                _context.Entry(materiel).State = EntityState.Detached;
//                throw new InvalidOperationException("Un matériel avec ce nom existe déjà.", ex);
//            }
//        }

//        public void Delete(Materiel materiel)
//        {
//            try
//            {
//                var connexion = _context.Database.GetDbConnection();
//                connexion.Open();

//                try
//                {
//                    using var commande = connexion.CreateCommand();
//                    commande.CommandText = "DELETE FROM Materiels WHERE Id = @Id";
//                    var param = commande.CreateParameter();
//                    param.ParameterName = "@Id";
//                    param.Value = materiel.Id;
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
//                throw new InvalidOperationException("Impossible de supprimer : ce matériel est utilisé dans un modèle.", ex);
//            }
//            catch (SqlException ex)
//            {
//                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
//            }
//        }
//    }
//}
