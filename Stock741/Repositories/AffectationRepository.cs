using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class AffectationRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public AffectationRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Affectation>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();
            return await BaseQuery(context)
                .OrderByDescending(a => a.Actif)
                .ThenByDescending(a => a.DateDebut)
                .ToListAsync();
        }

        public async Task<Affectation?> GetById(int id)
        {
            using var context = _contextFactory.CreateDbContext();
            return await BaseQuery(context)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Stock>> GetStocksDisponibles()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.Stocks
                .AsNoTracking()
                .Where(s => !context.Affectations.Any(a => a.StockId == s.Id && a.Actif))
                .OrderBy(s => s.Asset)
                .ThenBy(s => s.NumSerie)
                .Select(s => new Stock
                {
                    Id = s.Id,
                    Asset = s.Asset,
                    NumSerie = s.NumSerie,
                    StatutId = s.StatutId,
                    Statut = s.Statut == null ? null : new Statut
                    {
                        Id = s.Statut.Id,
                        Nom = s.Statut.Nom,
                        Type = s.Statut.Type
                    },
                    LieuId = s.LieuId,
                    Lieu = s.Lieu == null ? null : new Lieu
                    {
                        Id = s.Lieu.Id,
                        Nom = s.Lieu.Nom
                    },
                    ModeleId = s.ModeleId,
                    Modele = new Modele
                    {
                        Id = s.Modele.Id,
                        Nom = s.Modele.Nom,
                        CheminPhoto = s.Modele.CheminPhoto,
                        Materiel = new Materiel
                        {
                            Id = s.Modele.Materiel.Id,
                            Nom = s.Modele.Materiel.Nom
                        },
                        Marque = new Marque
                        {
                            Id = s.Modele.Marque.Id,
                            Nom = s.Modele.Marque.Nom
                        }
                    },
                    RowVersion = s.RowVersion
                })
                .ToListAsync();
        }

        public async Task Ajouter(Affectation affectation, string effectuePar)
        {
            using var context = _contextFactory.CreateDbContext();

            var dejaAffecte = await context.Affectations
                .AnyAsync(a => a.StockId == affectation.StockId && a.Actif);

            if (dejaAffecte)
                throw new InvalidOperationException("Ce matériel possède déjà une affectation active.");

            var stock = await context.Stocks.FirstOrDefaultAsync(s => s.Id == affectation.StockId);
            if (stock == null)
                throw new InvalidOperationException("Le matériel sélectionné n'existe plus.");

            var ancienStatutId = stock.StatutId;
            var ancienLieuId = stock.LieuId;
            var statutAffecte = await context.Statuts
                .Where(s => s.Nom.ToLower().Contains("affect"))
                .OrderBy(s => s.Nom)
                .FirstOrDefaultAsync();

            affectation.DateFin = null;
            affectation.Actif = true;
            NormaliserTextes(affectation);

            context.Affectations.Add(affectation);

            if (statutAffecte != null)
                stock.StatutId = statutAffecte.Id;

            await context.SaveChangesAsync();

            context.HistoriqueMouvements.Add(new HistoriqueMouvement
            {
                StockId = stock.Id,
                AffectationId = affectation.Id,
                TypeMouvement = "Affectation",
                AncienStatutId = ancienStatutId,
                AncienLieuId = ancienLieuId,
                NouveauStatutId = stock.StatutId,
                NouveauLieuId = stock.LieuId,
                DateMouvement = DateTime.Now,
                EffectuePar = effectuePar,
                Commentaire = affectation.Commentaire
            });

            await context.SaveChangesAsync();
        }

        public async Task Modifier(Affectation affectation, string effectuePar)
        {
            try
            {
                using var context = _contextFactory.CreateDbContext();

                var tracked = await context.Affectations
                    .FirstOrDefaultAsync(a => a.Id == affectation.Id);

                if (tracked == null)
                    throw new DbUpdateConcurrencyException();

                context.Entry(tracked)
                    .Property(a => a.RowVersion)
                    .OriginalValue = affectation.RowVersion;

                tracked.UtilisateurId = affectation.UtilisateurId;
                tracked.EdsId = affectation.EdsId;
                tracked.EdsAutomatiqueId = affectation.EdsAutomatiqueId;
                tracked.OperateurId = affectation.OperateurId;
                tracked.ForfaitId = affectation.ForfaitId;
                tracked.DateDebut = affectation.DateDebut;
                tracked.DatePret = affectation.DatePret;
                tracked.NomAppareil = affectation.NomAppareil;
                tracked.AdresseIP = affectation.AdresseIP;
                tracked.MasqueIP = affectation.MasqueIP;
                tracked.PasserelleIP = affectation.PasserelleIP;
                tracked.NomPC = affectation.NomPC;
                tracked.EdsPC = affectation.EdsPC;
                tracked.AncienPC = affectation.AncienPC;
                tracked.NumTelMobile = affectation.NumTelMobile;
                tracked.Motif = affectation.Motif;
                tracked.Commentaire = affectation.Commentaire;

                NormaliserTextes(tracked);

                context.HistoriqueMouvements.Add(new HistoriqueMouvement
                {
                    StockId = tracked.StockId,
                    AffectationId = tracked.Id,
                    TypeMouvement = "Modification",
                    DateMouvement = DateTime.Now,
                    EffectuePar = effectuePar,
                    Commentaire = "Modification de l'affectation"
                });

                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("Cette affectation a été modifiée ou supprimée par un autre utilisateur. Veuillez actualiser la vue.");
            }
        }

        public async Task Retourner(int affectationId, string effectuePar)
        {
            using var context = _contextFactory.CreateDbContext();

            var affectation = await context.Affectations
                .Include(a => a.Stock)
                .FirstOrDefaultAsync(a => a.Id == affectationId);

            if (affectation == null)
                throw new InvalidOperationException("L'affectation sélectionnée n'existe plus.");

            if (!affectation.Actif)
                throw new InvalidOperationException("Cette affectation est déjà clôturée.");

            var ancienStatutId = affectation.Stock?.StatutId;
            var ancienLieuId = affectation.Stock?.LieuId;
            var statutStock = await context.Statuts
                .Where(s => s.Nom.ToLower() == "stock")
                .FirstOrDefaultAsync();

            affectation.Actif = false;
            affectation.DateFin = DateTime.Now;

            if (affectation.Stock != null && statutStock != null)
                affectation.Stock.StatutId = statutStock.Id;

            context.HistoriqueMouvements.Add(new HistoriqueMouvement
            {
                StockId = affectation.StockId,
                AffectationId = affectation.Id,
                TypeMouvement = "Retour",
                AncienStatutId = ancienStatutId,
                AncienLieuId = ancienLieuId,
                NouveauStatutId = affectation.Stock?.StatutId,
                NouveauLieuId = affectation.Stock?.LieuId,
                AncienUtilisateurId = affectation.UtilisateurId,
                AncienEdsId = affectation.EdsId,
                AncienNomPC = affectation.NomPC,
                AncienNomAppareil = affectation.NomAppareil,
                AncienAdresseIP = affectation.AdresseIP,
                AncienMasqueIP = affectation.MasqueIP,
                AnciennePasserelle = affectation.PasserelleIP,
                DateMouvement = DateTime.Now,
                EffectuePar = effectuePar,
                Commentaire = "Retour du matériel"
            });

            await context.SaveChangesAsync();
        }

        private static IQueryable<Affectation> BaseQuery(AppDbContext context)
        {
            return context.Affectations
                .AsNoTracking()
                .Include(a => a.Stock)
                    .ThenInclude(s => s.Modele)
                        .ThenInclude(m => m.Marque)
                .Include(a => a.Stock)
                    .ThenInclude(s => s.Modele)
                        .ThenInclude(m => m.Materiel)
                .Include(a => a.Stock)
                    .ThenInclude(s => s.Statut)
                .Include(a => a.Stock)
                    .ThenInclude(s => s.Lieu)
                .Include(a => a.Utilisateur)
                .Include(a => a.Eds)
                .Include(a => a.EdsAutomatique)
                .Include(a => a.Operateur)
                .Include(a => a.Forfait);
        }

        private static void NormaliserTextes(Affectation affectation)
        {
            affectation.NomAppareil ??= string.Empty;
            affectation.AdresseIP ??= string.Empty;
            affectation.MasqueIP ??= string.Empty;
            affectation.PasserelleIP ??= string.Empty;
            affectation.NomPC ??= string.Empty;
            affectation.EdsPC ??= string.Empty;
            affectation.AncienPC ??= string.Empty;
            affectation.NumTelMobile ??= string.Empty;
            affectation.Motif ??= string.Empty;
            affectation.Commentaire ??= string.Empty;
        }
    }
}
