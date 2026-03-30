using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Stock741.Models
{
    public class Produit : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _nom;
        public string Nom
        {
            get => _nom;
            set { _nom = value; OnPropertyChanged(); }
        }

        private int _quantite;
        public int Quantite
        {
            get => _quantite;
            set { _quantite = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}