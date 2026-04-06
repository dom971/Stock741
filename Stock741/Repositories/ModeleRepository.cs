using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class ModeleRepository
    {
        private readonly AppDbContext _context;

        public ModeleRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Modele> GetAll()
        {
            return _context.Modeles
                .AsNoTracking()
                .Include(m => m.Marque)
                .Include(m => m.Materiel)
                    .ThenInclude(m => m.Fiche)
                .OrderBy(m => m.Nom)
                .ToList();
        }

        public void Add(Modele modele)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = @"INSERT INTO Modeles (Nom, CheminPhoto, Actif, MarqueId, MaterielId) 
                                            VALUES (@Nom, @CheminPhoto, @Actif, @MarqueId, @MaterielId); 
                                            SELECT SCOPE_IDENTITY();";
                    var p1 = commande.CreateParameter();
                    p1.ParameterName = "@Nom";
                    p1.Value = modele.Nom;
                    commande.Parameters.Add(p1);
                    var p2 = commande.CreateParameter();
                    p2.ParameterName = "@CheminPhoto";
                    p2.Value = modele.CheminPhoto ?? (object)DBNull.Value;
                    commande.Parameters.Add(p2);
                    var p3 = commande.CreateParameter();
                    p3.ParameterName = "@Actif";
                    p3.Value = modele.Actif;
                    commande.Parameters.Add(p3);
                    var p4 = commande.CreateParameter();
                    p4.ParameterName = "@MarqueId";
                    p4.Value = modele.MarqueId;
                    commande.Parameters.Add(p4);
                    var p5 = commande.CreateParameter();
                    p5.ParameterName = "@MaterielId";
                    p5.Value = modele.MaterielId;
                    commande.Parameters.Add(p5);
                    var id = commande.ExecuteScalar();
                    modele.Id = Convert.ToInt32(id);
                }
                finally
                {
                    connexion.Close();
                }
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                throw new InvalidOperationException("Un modèle avec ce nom existe déjà.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de l'ajout.", ex);
            }
        }

        public void Update(Modele modele)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = @"UPDATE Modeles 
                                            SET Nom = @Nom, CheminPhoto = @CheminPhoto, 
                                                Actif = @Actif, MarqueId = @MarqueId, 
                                                MaterielId = @MaterielId 
                                            WHERE Id = @Id";
                    var p1 = commande.CreateParameter();
                    p1.ParameterName = "@Nom";
                    p1.Value = modele.Nom;
                    commande.Parameters.Add(p1);
                    var p2 = commande.CreateParameter();
                    p2.ParameterName = "@CheminPhoto";
                    p2.Value = modele.CheminPhoto ?? (object)DBNull.Value;
                    commande.Parameters.Add(p2);
                    var p3 = commande.CreateParameter();
                    p3.ParameterName = "@Actif";
                    p3.Value = modele.Actif;
                    commande.Parameters.Add(p3);
                    var p4 = commande.CreateParameter();
                    p4.ParameterName = "@MarqueId";
                    p4.Value = modele.MarqueId;
                    commande.Parameters.Add(p4);
                    var p5 = commande.CreateParameter();
                    p5.ParameterName = "@MaterielId";
                    p5.Value = modele.MaterielId;
                    commande.Parameters.Add(p5);
                    var p6 = commande.CreateParameter();
                    p6.ParameterName = "@Id";
                    p6.Value = modele.Id;
                    commande.Parameters.Add(p6);
                    var lignesAffectees = commande.ExecuteNonQuery();
                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Ce modèle a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
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
                throw new InvalidOperationException("Un modèle avec ce nom existe déjà.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la modification.", ex);
            }
        }

        public void Delete(Modele modele)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "DELETE FROM Modeles WHERE Id = @Id";
                    var param = commande.CreateParameter();
                    param.ParameterName = "@Id";
                    param.Value = modele.Id;
                    commande.Parameters.Add(param);
                    var lignesAffectees = commande.ExecuteNonQuery();
                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Ce modèle a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
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
                throw new InvalidOperationException("Impossible de supprimer : ce modèle est utilisé.", ex);
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
//    public class ModeleRepository
//    {
//        private readonly AppDbContext _context;

//        public ModeleRepository(AppDbContext context)
//        {
//            _context = context;
//        }

//        public List<Modele> GetAll()
//        {
//            //return _context.Modeles
//            //   .Include(m => m.Marque)
//            //   .Include(m => m.Materiel)
//            //   .OrderBy(m => m.Nom)
//            //   .ToList();

//            return _context.Modeles
//                .AsNoTracking()  
//                .Include(m => m.Marque)
//                .Include(m => m.Materiel)
//                    .ThenInclude(m => m.Fiche)
//                .OrderBy(m => m.Nom)
//                .ToList();

//        }

//        public void Add(Modele modele)
//        {
//            _context.Modeles.Add(modele);
//            try
//            {
//                _context.SaveChanges();
//            }
//            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
//                                                (ex.InnerException as SqlException)?.Number == 2627)
//            {
//                _context.Entry(modele).State = EntityState.Detached;
//                throw new InvalidOperationException("Un modèle avec ce nom existe déjà.", ex);
//            }
//        }

//        public void Update(Modele modele)
//        {
//            _context.Modeles.Update(modele);
//            try
//            {
//                _context.SaveChanges();
//            }
//            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
//                                                (ex.InnerException as SqlException)?.Number == 2627)
//            {
//                _context.Entry(modele).State = EntityState.Detached;
//                throw new InvalidOperationException("Un modèle avec ce nom existe déjà.", ex);
//            }
//        }

//        public void Delete(Modele modele)
//        {
//            var tracked = _context.Modeles.Find(modele.Id);
//            if (tracked == null)
//                throw new InvalidOperationException("Modèle introuvable.");

//            _context.Modeles.Remove(tracked);
//            try
//            {
//                _context.SaveChanges();
//            }
//            catch (DbUpdateException ex)
//            {
//                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
//            }
//        }
//    }
//}