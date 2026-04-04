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
            //return _context.Modeles
            //   .Include(m => m.Marque)
            //   .Include(m => m.Materiel)
            //   .OrderBy(m => m.Nom)
            //   .ToList();

            return _context.Modeles
                .Include(m => m.Marque)
                .Include(m => m.Materiel)
                    .ThenInclude(m => m.Fiche)
                .OrderBy(m => m.Nom)
                .ToList();

        }

        public void Add(Modele modele)
        {
            _context.Modeles.Add(modele);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(modele).State = EntityState.Detached;
                throw new InvalidOperationException("Un modèle avec ce nom existe déjà.", ex);
            }
        }

        public void Update(Modele modele)
        {
            _context.Modeles.Update(modele);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex) when ((ex.InnerException as SqlException)?.Number == 2601 ||
                                                (ex.InnerException as SqlException)?.Number == 2627)
            {
                _context.Entry(modele).State = EntityState.Detached;
                throw new InvalidOperationException("Un modèle avec ce nom existe déjà.", ex);
            }
        }

        public void Delete(Modele modele)
        {
            var tracked = _context.Modeles.Find(modele.Id);
            if (tracked == null)
                throw new InvalidOperationException("Modèle introuvable.");

            _context.Modeles.Remove(tracked);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Erreur lors de la suppression.", ex);
            }
        }
    }
}