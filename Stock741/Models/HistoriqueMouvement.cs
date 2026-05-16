using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stock741.Models
{
    public class HistoriqueMouvement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StockId { get; set; }
        [ForeignKey(nameof(StockId))]
        public Stock Stock { get; set; }

        public int? AffectationId { get; set; }
        [ForeignKey(nameof(AffectationId))]
        public Affectation Affectation { get; set; }

        [MaxLength(20)]
        public string TypeMouvement { get; set; }

        // Ancien statut
        public int? AncienStatutId { get; set; }
        [ForeignKey(nameof(AncienStatutId))]
        public Statut AncienStatut { get; set; }

        // Ancien lieu
        public int? AncienLieuId { get; set; }
        [ForeignKey(nameof(AncienLieuId))]
        public Lieu AncienLieu { get; set; }

        // Ancien utilisateur
        public int? AncienUtilisateurId { get; set; }
        [ForeignKey(nameof(AncienUtilisateurId))]
        public Utilisateur AncienUtilisateur { get; set; }

        // Ancien EDS
        public int? AncienEdsId { get; set; }
        [ForeignKey(nameof(AncienEdsId))]
        public Eds AncienEds { get; set; }

        // Ancien PC
        [MaxLength(12)]
        public string AncienNomPC { get; set; }

        // Ancien appareil réseau
        [MaxLength(100)]
        public string AncienNomAppareil { get; set; }

        // Ancien réseau
        [MaxLength(50)]
        public string AncienAdresseIP { get; set; }

        [MaxLength(50)]
        public string AncienMasqueIP { get; set; }

        [MaxLength(50)]
        public string AnciennePasserelle { get; set; }

        // Traçabilité
        public DateTime DateMouvement { get; set; } = DateTime.Now;

        [MaxLength(100)]
        public string EffectuePar { get; set; }

        [MaxLength(500)]
        public string Commentaire { get; set; }
    }
}
