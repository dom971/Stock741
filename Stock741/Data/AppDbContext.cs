using Microsoft.EntityFrameworkCore;
using Stock741.Models;

namespace Stock741.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Produit> Produits { get; set; }
        public DbSet<Marque> Marques { get; set; } // <- ajoute simplement ça

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            //options.UseSqlServer("Server=ULYSSESHOP\\SQLEXPRESS;Database=Stock741Db;User Id=Stock741User;Password=123;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;");
            options.UseSqlServer("Server=ULYSSESHOP\\SQLEXPRESS;Database=Stock741Db;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produit>()
                .HasOne(p => p.Marque)
                .WithMany() // pas de navigation inverse
                .HasForeignKey(p => p.MarqueId)
                .OnDelete(DeleteBehavior.Restrict); // 🔥 IMPORTANT

            base.OnModelCreating(modelBuilder);
        }


    }
}