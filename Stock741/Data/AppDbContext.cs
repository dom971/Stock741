using Microsoft.EntityFrameworkCore;
using Stock741.Models;



namespace Stock741.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Marque> Marques { get; set; }
        public DbSet<Materiel> Materiels { get; set; }

        public DbSet<Modele> Modeles { get; set; }


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Marque>()
                .Property(m => m.Actif)
                .HasDefaultValue(true);

            modelBuilder.Entity<Materiel>()
                .Property(m => m.Actif)
                .HasDefaultValue(true);

            modelBuilder.Entity<Modele>()
        .       HasOne(m => m.Marque)
                .WithMany()
                .HasForeignKey(m => m.MarqueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Modele>()
                .HasOne(m => m.Materiel)
                .WithMany()
                .HasForeignKey(m => m.MaterielId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
        }
    }

}