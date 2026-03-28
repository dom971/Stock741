

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Stock741.Model
{
    public class Marque : INotifyPropertyChanged
    {
        // =====================================================
        // 🔔 INOTIFYPROPERTYCHANGED
        // =====================================================
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

        // =====================================================
        // 📋 PROPRIÉTÉS
        // =====================================================
        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        private string _nom;
        public string Nom
        {
            get => _nom;
            set { _nom = value; OnPropertyChanged(); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private DateTime _dateCreation;
        public DateTime DateCreation
        {
            get => _dateCreation;
            set { _dateCreation = value; OnPropertyChanged(); }
        }

        private DateTime? _dateModification;
        public DateTime? DateModification
        {
            get => _dateModification;
            set { _dateModification = value; OnPropertyChanged(); }
        }

        private bool _actif;
        public bool Actif
        {
            get => _actif;
            set { _actif = value; OnPropertyChanged(); }
        }

        private byte[] _version;
        public byte[] Version
        {
            get => _version;
            set { _version = value; OnPropertyChanged(); }
        }
    }
}



//using Stock741;
//using Stock741.Model;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;



//namespace Stock741.Model
//{
//    public class Marque
//    {
//        public int Id { get; set; }
//        public string Nom { get; set; }
//        public string Description { get; set; }
//        public DateTime DateCreation { get; set; }
//        public DateTime? DateModification { get; set; }
//        public bool Actif { get; set; }
//        public byte[] Version { get; set; } // pour ROWVERSION
//    }
//}
