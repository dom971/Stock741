using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Stock741.Models
{
    [Index(nameof(Nom), IsUnique = true)]
    public class Operateur
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}