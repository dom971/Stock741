using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;

namespace Stock741.Repositories
{
    public class HistoriqueMouvementRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public HistoriqueMouvementRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<HistoriqueMouvement>> GetAll()
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.HistoriqueMouvements
                .AsNoTracking()
                .Include(h => h.Stock)
                    .ThenInclude(s => s.Modele)
                        .ThenInclude(m => m.Materiel)
                .Include(h => h.Stock)
                    .ThenInclude(s => s.Modele)
                        .ThenInclude(m => m.Marque)
                .Include(h => h.Affectation)
                .Include(h => h.AncienStatut)
                .Include(h => h.AncienLieu)
                .Include(h => h.NouveauStatut)
                .Include(h => h.NouveauLieu)
                .Include(h => h.AncienUtilisateur)
                .Include(h => h.AncienEds)
                .Include(h => h.NouveauUtilisateur)
                .Include(h => h.NouveauEds)
                .OrderByDescending(h => h.DateMouvement)
                .ToListAsync();
        }
    }
}
