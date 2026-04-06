using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class FournisseurRepository
    {
        private readonly AppDbContext _context;

        public FournisseurRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Fournisseur> GetAll()
        {
            return _context.Fournisseurs
                .AsNoTracking()
                .OrderBy(f => f.Nom)
                .ToList();
        }

        public void Add(Fournisseur fournisseur)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "INSERT INTO Fournisseurs (Nom) VALUES (@Nom); SELECT SCOPE_IDENTITY();";
                    var p1 = commande.CreateParameter();
                    p1.ParameterName = "@Nom";
                    p1.Value = fournisseur.Nom;
                    commande.Parameters.Add(p1);
                    var id = commande.ExecuteScalar();
                    fournisseur.Id = Convert.ToInt32(id);
                }
                finally
                {
                    connexion.Close();
                }
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                throw new InvalidOperationException("Un fournisseur avec ce nom existe déjà.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de l'ajout.", ex);
            }
        }

        public void Update(Fournisseur fournisseur)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "UPDATE Fournisseurs SET Nom = @Nom WHERE Id = @Id";
                    var p1 = commande.CreateParameter();
                    p1.ParameterName = "@Nom";
                    p1.Value = fournisseur.Nom;
                    commande.Parameters.Add(p1);
                    var p2 = commande.CreateParameter();
                    p2.ParameterName = "@Id";
                    p2.Value = fournisseur.Id;
                    commande.Parameters.Add(p2);
                    var lignesAffectees = Convert.ToInt32(commande.ExecuteNonQuery());
                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Ce fournisseur a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
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
                throw new InvalidOperationException("Un fournisseur avec ce nom existe déjà.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la modification.", ex);
            }
        }

        //public void Add(Fournisseur fournisseur)
        //{
        //    _context.Fournisseurs.Add(fournisseur);
        //    try
        //    {
        //        _context.SaveChanges();
        //    }
        //    catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
        //                                        (ex.InnerException as SqlException)?.Number == 2627)
        //    {
        //        _context.Entry(fournisseur).State = EntityState.Detached;
        //        throw new InvalidOperationException("Un fournisseur avec ce nom existe déjà.", ex);
        //    }
        //}

        //public void Update(Fournisseur fournisseur)
        //{
        //    var connexion = _context.Database.GetDbConnection();
        //    connexion.Open();
        //    try
        //    {
        //        using var commande = connexion.CreateCommand();
        //        commande.CommandText = "UPDATE Fournisseurs SET Nom = @Nom WHERE Id = @Id";
        //        var p1 = commande.CreateParameter();
        //        p1.ParameterName = "@Nom";
        //        p1.Value = fournisseur.Nom;
        //        commande.Parameters.Add(p1);
        //        var p2 = commande.CreateParameter();
        //        p2.ParameterName = "@Id";
        //        p2.Value = fournisseur.Id;
        //        commande.Parameters.Add(p2);
        //        commande.ExecuteNonQuery();
        //    }
        //    finally
        //    {
        //        connexion.Close();
        //    }
        //}

        public void Delete(Fournisseur fournisseur)
        {
            try
            {
                var connexion = _context.Database.GetDbConnection();
                connexion.Open();

                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "DELETE FROM Fournisseurs WHERE Id = @Id";
                    var param = commande.CreateParameter();
                    param.ParameterName = "@Id";
                    param.Value = fournisseur.Id;
                    commande.Parameters.Add(param);
                    var lignesAffectees = commande.ExecuteNonQuery();

                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Ce fournisseur a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
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
                throw new InvalidOperationException("Impossible de supprimer : ce fournisseur est utilisé.", ex);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
            }
        }

        //public void Update(Fournisseur fournisseur)
        //{
        //    _context.Fournisseurs.Update(fournisseur);
        //    try
        //    {
        //        _context.SaveChanges();
        //    }
        //    catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
        //                                        (ex.InnerException as SqlException)?.Number == 2627)
        //    {
        //        _context.Entry(fournisseur).State = EntityState.Detached;
        //        throw new InvalidOperationException("Un fournisseur avec ce nom existe déjà.", ex);
        //    }
        //}

        //public void Delete(Fournisseur fournisseur)
        //{
        //    try
        //    {
        //        var connexion = _context.Database.GetDbConnection();
        //        connexion.Open();

        //        try
        //        {
        //            using var commande = connexion.CreateCommand();
        //            commande.CommandText = "DELETE FROM Fournisseurs WHERE Id = @Id";
        //            var param = commande.CreateParameter();
        //            param.ParameterName = "@Id";
        //            param.Value = fournisseur.Id;
        //            commande.Parameters.Add(param);
        //            commande.ExecuteNonQuery();
        //        }
        //        finally
        //        {
        //            connexion.Close();
        //        }
        //    }
        //    catch (SqlException ex) when (ex.Number == 547)
        //    {
        //        throw new InvalidOperationException("Impossible de supprimer : ce fournisseur est utilisé.", ex);
        //    }
        //    catch (SqlException ex)
        //    {
        //        throw new InvalidOperationException("Erreur lors de la suppression.", ex);
        //    }
        //}
    }
}