using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stock741.Models
{
    [Index(nameof(Nom), IsUnique = true)]
    public class Modele
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string CheminPhoto { get; set; }

        public bool Actif { get; set; } = true;

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public int MarqueId { get; set; }
        [ForeignKey(nameof(MarqueId))]
        public Marque Marque { get; set; }

        public int MaterielId { get; set; }
        [ForeignKey(nameof(MaterielId))]
        public Materiel Materiel { get; set; }
    }
}
