using Stock741.Data;
using Stock741.Models;
using System.Collections.Generic;
using System.Linq;

namespace Stock741.Repositories
{
    public class ProduitRepository
    {
        private readonly AppDbContext _context;

        public ProduitRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Produit> GetAll() => _context.Produits.ToList();

        public void Add(Produit produit)
        {
            _context.Produits.Add(produit);
            _context.SaveChanges();
        }

        public void Update(Produit produit)
        {
            _context.SaveChanges();
        }

        public void Delete(Produit produit)
        {
            _context.Produits.Remove(produit);
            _context.SaveChanges();
        }
    }
}