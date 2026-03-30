using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Stock741.Models
{
    public class Marque : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _nom;
        public string Nom
        {
            get => _nom;
            set { _nom = value; OnPropertyChanged(); }
        }

        private bool _actif = true;
        public bool Actif
        {
            get => _actif;
            set { _actif = value; OnPropertyChanged(); }
        }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
