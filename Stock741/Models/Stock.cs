
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stock741.Models
{
    [Index(nameof(Asset), IsUnique = true)]
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        // Identification
        [MaxLength(8)]
        public string? Asset { get; set; }

        // Réception
        public DateTime Date { get; set; } = DateTime.Now;

        [MaxLength(9)]
        public string? NumReception { get; set; } = "0";

        public int? StatutId { get; set; }
        [ForeignKey(nameof(StatutId))]
        public Statut? Statut { get; set; }

        public int? LieuId { get; set; }
        [ForeignKey(nameof(LieuId))]
        public Lieu? Lieu { get; set; }       

        [MaxLength(50)]
        public string? Colis { get; set; } = "0";

        // Références (FK)
        [Required]
        public int ModeleId { get; set; }
        [ForeignKey(nameof(ModeleId))]
        public Modele? Modele { get; set; }              

        public int? FournisseurId { get; set; }
        [ForeignKey(nameof(FournisseurId))]
        public Fournisseur? Fournisseur { get; set; }

        [Required]
        [MaxLength(100)]
        public string NumSerie { get; set; }

        public int Qte { get; set; } = 1;

        public bool SousGarantie { get; set; } = true;

        public DateTime? Garantie { get; set; }

        public int? SystemeId { get; set; }
        [ForeignKey(nameof(SystemeId))]
        public Systeme? Systeme { get; set; }

        // Téléphone / Dongle 4G
        [MaxLength(50)]
        public string? NumSim { get; set; }

        [MaxLength(50)]
        public string? Imei1 { get; set; }

        [MaxLength(50)]
        public string? Imei2 { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [NotMapped]
        public bool AffectationActive { get; set; }

        [NotMapped]
        public bool ADejaEteAffecte { get; set; }

        [NotMapped]
        public string EtatAffectation =>
            AffectationActive ? "Actif" :
            ADejaEteAffecte ? "Historique" :
            string.Empty;
    }
}
