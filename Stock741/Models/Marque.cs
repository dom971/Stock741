using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Stock741.Models
{
    [Index(nameof(Nom), IsUnique = true)]  // ← unicité
    public class Marque : INotifyPropertyChanged
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }

        public bool Actif { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<Produit> Produits { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Runtime.CompilerServices;

//namespace Stock741.Models
//{
//    public class Marque : INotifyPropertyChanged
//    {
//        private int _id;
//        private string _nom;
//        private bool _actif;
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

//        [Timestamp]
//        public byte[] RowVersion
//        {
//            get => _rowVersion;
//            set { _rowVersion = value; OnPropertyChanged(); }
//        }

//        // Navigation vers les produits
//        public ICollection<Produit> Produits { get; set; }

//        // INotifyPropertyChanged
//        public event PropertyChangedEventHandler PropertyChanged;
//        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//}


//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Stock741.Models
//{
//    public class Marque 
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        public string Nom { get; set; }

//        public bool Actif { get; set; } = true;

//        [Timestamp]
//        public byte[] RowVersion { get; set; }

//        // Navigation vers les produits
//        public ICollection<Produit> Produits { get; set; }
//    }
//}

//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.Runtime.CompilerServices;

//namespace Stock741.Models
//{
//    public class Marque : INotifyPropertyChanged
//    {
//        public int Id { get; set; }

//        private string _nom;
//        public string Nom
//        {
//            get => _nom;
//            set { _nom = value; OnPropertyChanged(); }
//        }

//        private bool _actif = true;
//        public bool Actif
//        {
//            get => _actif;
//            set { _actif = value; OnPropertyChanged(); }
//        }

//        [Timestamp]
//        public byte[] RowVersion { get; set; }

//        public event PropertyChangedEventHandler PropertyChanged;
//        protected void OnPropertyChanged([CallerMemberName] string name = null)
//            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
//    }
//}
