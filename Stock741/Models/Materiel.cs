using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Stock741.Models
{
    [Index(nameof(Nom), IsUnique = true)]
    public class Materiel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        public bool Actif { get; set; } = true;

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
