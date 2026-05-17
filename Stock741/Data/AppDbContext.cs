using Microsoft.EntityFrameworkCore;
using Stock741.Models;



namespace Stock741.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Marque> Marques { get; set; }
        public DbSet<Materiel> Materiels { get; set; }
        public DbSet<Modele> Modeles { get; set; }
        public DbSet<Lieu> Lieux { get; set; }
        public DbSet<Fiche> Fiches { get; set; }
        public DbSet<Statut> Statuts { get; set; }
        public DbSet<Fournisseur> Fournisseurs { get; set; }
        public DbSet<Operateur> Operateurs { get; set; }
        public DbSet<Forfait> Forfaits { get; set; }
        public DbSet<Eds> Eds { get; set; }
        public DbSet<EdsLiaison> EdsLiaisons { get; set; }
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Systeme> Systemes { get; set; }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Affectation> Affectations { get; set; }
        public DbSet<HistoriqueMouvement> HistoriqueMouvements { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Marque>()
                .Property(m => m.Actif)
                .HasDefaultValue(true);

            modelBuilder.Entity<Materiel>()
                .Property(m => m.Actif)
                .HasDefaultValue(true);

            modelBuilder.Entity<Materiel>()
                .HasOne(m => m.Fiche)
                .WithMany()
                .HasForeignKey(m => m.FicheId)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<Forfait>()
                .HasOne(f => f.Operateur)
                .WithMany()
                .HasForeignKey(f => f.OperateurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Eds>()
                .ToTable("Eds");

            modelBuilder.Entity<EdsLiaison>()
                .HasOne(el => el.Eds)
                .WithMany()
                .HasForeignKey(el => el.EdsId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Utilisateur>()
                .HasIndex(u => u.IdWindows)
                .IsUnique();

            //Stock
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Modele)
                .WithMany()
                .HasForeignKey(s => s.ModeleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Fournisseur)
                .WithMany()
                .HasForeignKey(s => s.FournisseurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Statut)
                .WithMany()
                .HasForeignKey(s => s.StatutId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Lieu)
                .WithMany()
                .HasForeignKey(s => s.LieuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Systeme)
                .WithMany()
                .HasForeignKey(s => s.SystemeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stock>()
                .Property(s => s.SousGarantie)
                .HasDefaultValue(true);

            modelBuilder.Entity<Stock>()
                .Property(s => s.Colis)
                .HasDefaultValue("0");

            modelBuilder.Entity<Stock>()
                .Property(s => s.NumReception)
                .HasDefaultValue("0");

            // Affectation
            modelBuilder.Entity<Affectation>()
                .HasOne(a => a.Stock)
                .WithMany()
                .HasForeignKey(a => a.StockId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Affectation>()
                .HasOne(a => a.Utilisateur)
                .WithMany()
                .HasForeignKey(a => a.UtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Affectation>()
                .HasOne(a => a.Eds)
                .WithMany()
                .HasForeignKey(a => a.EdsId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Affectation>()
                .HasOne(a => a.Operateur)
                .WithMany()
                .HasForeignKey(a => a.OperateurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Affectation>()
                .HasOne(a => a.Forfait)
                .WithMany()
                .HasForeignKey(a => a.ForfaitId)
                .OnDelete(DeleteBehavior.Restrict);

            // HistoriqueMouvement
            modelBuilder.Entity<HistoriqueMouvement>()
                .HasOne(h => h.Stock)
                .WithMany()
                .HasForeignKey(h => h.StockId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistoriqueMouvement>()
                .HasOne(h => h.Affectation)
                .WithMany()
                .HasForeignKey(h => h.AffectationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistoriqueMouvement>()
                .HasOne(h => h.AncienStatut)
                .WithMany()
                .HasForeignKey(h => h.AncienStatutId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistoriqueMouvement>()
                .HasOne(h => h.AncienLieu)
                .WithMany()
                .HasForeignKey(h => h.AncienLieuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistoriqueMouvement>()
                .HasOne(h => h.NouveauStatut)
                .WithMany()
                .HasForeignKey(h => h.NouveauStatutId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistoriqueMouvement>()
                .HasOne(h => h.NouveauLieu)
                .WithMany()
                .HasForeignKey(h => h.NouveauLieuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistoriqueMouvement>()
                .HasOne(h => h.AncienUtilisateur)
                .WithMany()
                .HasForeignKey(h => h.AncienUtilisateurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistoriqueMouvement>()
                .HasOne(h => h.AncienEds)
                .WithMany()
                .HasForeignKey(h => h.AncienEdsId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
        }
    }

}
