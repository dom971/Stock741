using Microsoft.EntityFrameworkCore;
using Stock741.Data;
using Stock741.Models;
using System.Collections.Generic;
using System.Linq;

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
            return _context.Marques.Include(m => m.Produits).ToList();
        }

        public void Add(Marque marque)
        {
            _context.Marques.Add(marque);
            _context.SaveChanges();
        }

        public void Update(Marque marque)
        {
            _context.Marques.Update(marque);
            _context.SaveChanges();
        }

        public void Delete(Marque marque)
        {
            _context.Marques.Remove(marque);
            _context.SaveChanges();
        }
    }
}

//using Stock741.Data;
//using Stock741.Models;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;

//namespace Stock741.Repositories
//{
//    public class MarqueRepository
//    {
//        private readonly AppDbContext _context;
//        public MarqueRepository(AppDbContext context) => _context = context;

//        public List<Marque> GetAll() => _context.Marques.ToList();

//        public void Add(Marque marque)
//        {
//            _context.Marques.Add(marque);
//            _context.SaveChanges();
//        }

//        public void Update(Marque marque)
//        {
//            try
//            {
//                _context.SaveChanges();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                throw new System.Exception("La marque a été modifiée par un autre utilisateur !");
//            }
//        }

//        public void Delete(Marque marque)
//        {
//            _context.Marques.Remove(marque);
//            _context.SaveChanges();
//        }
//    }
//}
