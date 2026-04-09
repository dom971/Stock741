using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class FournisseurRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public FournisseurRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Fournisseur>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Fournisseurs
                .AsNoTracking()
                .OrderBy(f => f.Nom)
                .ToListAsync();
        }

        public async Task Add(Fournisseur fournisseur)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var connexion = context.Database.GetDbConnection();
                await connexion.OpenAsync();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "INSERT INTO Fournisseurs (Nom) VALUES (@Nom); SELECT SCOPE_IDENTITY();";
                    var p1 = commande.CreateParameter();
                    p1.ParameterName = "@Nom";
                    p1.Value = fournisseur.Nom;
                    commande.Parameters.Add(p1);
                    var id = await commande.ExecuteScalarAsync();
                    fournisseur.Id = Convert.ToInt32(id);
                }
                finally
                {
                    await connexion.CloseAsync();
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

        public async Task Update(Fournisseur fournisseur)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var connexion = context.Database.GetDbConnection();
                await connexion.OpenAsync();
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
                    var lignesAffectees = Convert.ToInt32(await commande.ExecuteNonQueryAsync());
                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Ce fournisseur a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
                }
                finally
                {
                    await connexion.CloseAsync();
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

        public async Task Delete(Fournisseur fournisseur)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();
                var connexion = context.Database.GetDbConnection();
                await connexion.OpenAsync();
                try
                {
                    using var commande = connexion.CreateCommand();
                    commande.CommandText = "DELETE FROM Fournisseurs WHERE Id = @Id";
                    var param = commande.CreateParameter();
                    param.ParameterName = "@Id";
                    param.Value = fournisseur.Id;
                    commande.Parameters.Add(param);
                    var lignesAffectees = await commande.ExecuteNonQueryAsync();
                    if (lignesAffectees == 0)
                        throw new InvalidOperationException("Ce fournisseur a été supprimé par un autre utilisateur. Veuillez rafraîchir la vue.");
                }
                finally
                {
                    await connexion.CloseAsync();
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
    }
}