
using System.ComponentModel.DataAnnotations;

namespace Stock741.Models
{
    public class Systeme
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nom { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}