using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Stock741.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _nom;
        public string Nom
        {
            get => _nom;
            set { _nom = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

