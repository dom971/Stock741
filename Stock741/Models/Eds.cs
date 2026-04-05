using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Stock741.Models
{
    [Index(nameof(Cnx), IsUnique = true)]
    public class Eds
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Cnx { get; set; }

        [Required]
        public string Nom { get; set; }

        public string Adr1 { get; set; }
        public string Adr2 { get; set; }
        public string Adr3 { get; set; }
        public string Adr4 { get; set; }

        public string HorLundi { get; set; }
        public string HorMardi { get; set; }
        public string HorMercredi { get; set; }
        public string HorJeudi { get; set; }
        public string HorVendredi { get; set; }
        public string HorSamedi { get; set; }

        public string Geolocalisation { get; set; }
        public string MailContact { get; set; }

        public bool Actif { get; set; } = true;

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}