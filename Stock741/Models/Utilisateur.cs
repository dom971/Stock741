using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Stock741.Models
{
    [Index(nameof(IdWindows), IsUnique = true)]
    public class Utilisateur
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Societe { get; set; }

        [MaxLength(100)]
        public string IdUtilisateur { get; set; }

        [MaxLength(100)]
        public string Prenom { get; set; }

        [MaxLength(100)]
        public string Nom { get; set; }

        [MaxLength(200)]
        public string NomComplet { get; set; }

        [MaxLength(50)]
        public string TelephoneMobile { get; set; }

        [MaxLength(50)]
        public string TelephoneProfessionnel { get; set; }

        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(200)]
        public string Emplacement { get; set; }

        [MaxLength(200)]
        public string Departement { get; set; }

        [MaxLength(100)]
        public string Bureau { get; set; }

        [MaxLength(200)]
        public string Rue { get; set; }

        [MaxLength(20)]
        public string CodePostal { get; set; }

        [MaxLength(100)]
        public string Ville { get; set; }

        [MaxLength(10)]
        public string CodePays { get; set; }

        [MaxLength(100)]
        public string IdWindows { get; set; }

        public bool Vip { get; set; }

        public bool Actif { get; set; }

        [MaxLength(200)]
        public string Manager { get; set; }

        [MaxLength(100)]
        public string FuseauHoraire { get; set; }

        public DateTime? DateCreation { get; set; }

        [MaxLength(200)]
        public string CreePar { get; set; }

        [MaxLength(200)]
        public string MisAJourPar { get; set; }

        public DateTime? DateMiseAJour { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}