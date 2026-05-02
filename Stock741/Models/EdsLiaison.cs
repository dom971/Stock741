using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stock741.Models
{
    public class EdsLiaison
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Cible { get; set; }

        public int EdsId { get; set; }
        [ForeignKey(nameof(EdsId))]
        public Eds Eds { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}