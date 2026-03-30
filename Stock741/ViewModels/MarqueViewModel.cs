using System.Collections.ObjectModel;
using System.Windows.Input;
using Stock741.Models;
using Stock741.Repositories;
using Stock741.Commands;

namespace Stock741.ViewModels
{
    public class MarqueViewModel : BaseViewModel
    {
        private readonly MarqueRepository _repository;

        public ObservableCollection<Marque> Marques { get; set; }

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

        private Marque _marqueSelectionnee;
        public Marque MarqueSelectionnee
        {
            get => _marqueSelectionnee;
            set
            {
                _marqueSelectionnee = value;
                OnPropertyChanged();
                if (_marqueSelectionnee != null)
                {
                    Nom = _marqueSelectionnee.Nom;
                    Actif = _marqueSelectionnee.Actif;
                }
            }
        }

        public ICommand AjouterMarqueCommand { get; }
        public ICommand ModifierMarqueCommand { get; }
        public ICommand SupprimerMarqueCommand { get; }

        public MarqueViewModel(MarqueRepository repository)
        {
            _repository = repository;
            Marques = new ObservableCollection<Marque>(_repository.GetAll());

            AjouterMarqueCommand = new RelayCommand(_ => Ajouter());
            ModifierMarqueCommand = new RelayCommand(_ => Modifier());
            SupprimerMarqueCommand = new RelayCommand(_ => Supprimer());
        }

        private void Ajouter()
        {
            if (string.IsNullOrWhiteSpace(Nom)) return;

            var marque = new Marque { Nom = Nom, Actif = Actif };
            _repository.Add(marque);
            Marques.Add(marque);

            Nom = "";
            Actif = true;
        }

        private void Modifier()
        {
            if (MarqueSelectionnee == null) return;

            MarqueSelectionnee.Nom = Nom;
            MarqueSelectionnee.Actif = Actif;

            _repository.Update(MarqueSelectionnee);
        }

        private void Supprimer()
        {
            if (MarqueSelectionnee == null) return;

            _repository.Delete(MarqueSelectionnee);
            Marques.Remove(MarqueSelectionnee);
        }
    }
}
