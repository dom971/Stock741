using Microsoft.EntityFrameworkCore;
using Stock741.Models;

namespace Stock741.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Produit> Produits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=ULYSSESHOP\\SQLEXPRESS;Database=Stock741;User Id=Stock741User;Password=123;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");
        }
    }
}