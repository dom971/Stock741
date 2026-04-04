
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stock741.Models
{
    [Index(nameof(Nom), IsUnique = true)]
    public class Forfait
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        public bool Actif { get; set; } = true;

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public int OperateurId { get; set; }
        [ForeignKey(nameof(OperateurId))]
        public Operateur Operateur { get; set; }
    }
}