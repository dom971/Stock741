using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stock741.Models
{
    public class Affectation
    {
        [Key]
        public int Id { get; set; }

        // Références
        [Required]
        public int StockId { get; set; }
        [ForeignKey(nameof(StockId))]
        public Stock Stock { get; set; }

        public int? UtilisateurId { get; set; }
        [ForeignKey(nameof(UtilisateurId))]
        public Utilisateur Utilisateur { get; set; }

        public int? EdsId { get; set; }
        [ForeignKey(nameof(EdsId))]
        public Eds Eds { get; set; }

        public int? OperateurId { get; set; }
        [ForeignKey(nameof(OperateurId))]
        public Operateur Operateur { get; set; }

        public int? ForfaitId { get; set; }
        [ForeignKey(nameof(ForfaitId))]
        public Forfait Forfait { get; set; }

        // Dates
        public DateTime DateDebut { get; set; } = DateTime.Now;
        public DateTime? DateFin { get; set; }

        // PC
        [MaxLength(100)]
        public string NomAppareil { get; set; }

        [MaxLength(50)]
        public string AdresseIP { get; set; }

        [MaxLength(50)]
        public string MasqueIP { get; set; }

        [MaxLength(50)]
        public string PasserelleIP { get; set; }

        [MaxLength(12)]
        public string NomPC { get; set; }

        [MaxLength(3)]
        public string EdsPC { get; set; }

        [MaxLength(12)]
        public string AncienPC { get; set; }

        // Téléphone
        [MaxLength(20)]
        public string NumTelMobile { get; set; }

        // Gestion
        public string Motif { get; set; }
        public string Commentaire { get; set; }

        public bool Actif { get; set; } = true;

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}