using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Stock741.Models
{
    [Index(nameof(Nom), IsUnique = true)]  // ← unicité
    public class Produit : INotifyPropertyChanged
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        public bool Actif { get; set; }

        public int MarqueId { get; set; }
        public Marque Marque { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Runtime.CompilerServices;

//namespace Stock741.Models
//{
//    public class Produit : INotifyPropertyChanged
//    {
//        private int _id;
//        private string _nom;
//        private bool _actif;
//        private int _marqueId;
//        private Marque _marque;
//        private byte[] _rowVersion;

//        [Key]
//        public int Id
//        {
//            get => _id;
//            set { _id = value; OnPropertyChanged(); }
//        }

//        [Required]
//        public string Nom
//        {
//            get => _nom;
//            set { _nom = value; OnPropertyChanged(); }
//        }

//        public bool Actif
//        {
//            get => _actif;
//            set { _actif = value; OnPropertyChanged(); }
//        }

//        // Foreign key vers Marque
//        public int MarqueId
//        {
//            get => _marqueId;
//            set { _marqueId = value; OnPropertyChanged(); }
//        }

//        // Navigation
//        public Marque Marque
//        {
//            get => _marque;
//            set { _marque = value; OnPropertyChanged(); }
//        }

//        [Timestamp]
//        public byte[] RowVersion
//        {
//            get => _rowVersion;
//            set { _rowVersion = value; OnPropertyChanged(); }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;
//        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//}


//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Stock741.Models
//{
//    public class Produit
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        public string Nom { get; set; }

//        // Navigation vers la marque
//        [Required]
//        public int MarqueId { get; set; }
//        public Marque Marque { get; set; }

//        public bool Actif { get; set; } = true;

//        // Pour la gestion de la concurrence
//        [Timestamp]
//        public byte[] RowVersion { get; set; }
//    }
//}


//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Runtime.CompilerServices;

//namespace Stock741.Models
//{
//    public class Produit : INotifyPropertyChanged
//    {
//        public int Id { get; set; }

//        private string _nom;
//        public string Nom
//        {
//            get => _nom;
//            set { _nom = value; OnPropertyChanged(); }
//        }

//        // 🔗 Clé étrangère vers Marque
//        public int? MarqueId { get; set; }

//        // Navigation vers Marque (optionnelle mais utile)
//        public Marque Marque { get; set; }

//        // Nouveau champ
//        private bool _actif = true;
//        public bool Actif
//        {
//            get => _actif;
//            set { _actif = value; OnPropertyChanged(); }
//        }

//        // (Optionnel) Concurrence optimiste
//        [Timestamp]
//        public byte[] RowVersion { get; set; }

//        public event PropertyChangedEventHandler PropertyChanged;

//        protected void OnPropertyChanged([CallerMemberName] string name = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
//        }
//    }
//}