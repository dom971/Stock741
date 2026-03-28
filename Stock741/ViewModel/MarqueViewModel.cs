using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Data.SqlClient;
using Stock741.Model;

namespace Stock741
{
    public class MarqueViewModel : INotifyPropertyChanged
    {
        // =====================================================
        // 🔔 INOTIFYPROPERTYCHANGED
        // =====================================================
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string p = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

        // =====================================================
        // ⚙️ CONNEXION
        // =====================================================
        private readonly string _connectionString =
            "Server=ULYSSESHOP\\SQLEXPRESS;Database=Stock741;User Id=Stock741User;Password=123;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        // =====================================================
        // 📋 PROPRIÉTÉS
        // =====================================================
        public ObservableCollection<Marque> Marques { get; set; }
            = new ObservableCollection<Marque>();

        private Marque _selectedMarque;
        public Marque SelectedMarque
        {
            get => _selectedMarque;
            set
            {
                _selectedMarque = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PeutAjouter));
                OnPropertyChanged(nameof(PeutModifier));

            }
        }

        // Actif uniquement si c'est une nouvelle marque (pas encore en base)
        public bool PeutAjouter => SelectedMarque != null && SelectedMarque.Id == 0;

        // Actif uniquement si la marque existe déjà en base
        public bool PeutModifier => SelectedMarque != null && SelectedMarque.Id > 0;


        // =====================================================
        // 🔄 LOAD
        // =====================================================
        public void LoadMarques()
        {
            Marques.Clear();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Nom, Description, DateCreation, DateModification, Actif, Version FROM Marque ORDER BY Nom";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Marques.Add(new Marque
                    {
                        Id = reader.GetInt32(0),
                        Nom = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        DateCreation = reader.GetDateTime(3),
                        DateModification = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                        Actif = reader.GetBoolean(5),
                        Version = (byte[])reader[6]
                    });
                }
            }
        }

        // =====================================================
        // ➕ ADD
        // =====================================================
        public void AddMarque(Marque m)
        {
            if (m == null || string.IsNullOrWhiteSpace(m.Nom))
            {
                MessageBox.Show("⚠️ Le nom est obligatoire !");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Marque (Nom, Description) VALUES (@Nom, @Description)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nom", m.Nom.Trim());
                    cmd.Parameters.AddWithValue("@Description",
                        string.IsNullOrWhiteSpace(m.Description)
                            ? (object)DBNull.Value
                            : m.Description.Trim());
                    cmd.ExecuteNonQuery();
                }
                LoadMarques();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    MessageBox.Show("⚠️ Cette marque existe déjà !");
                else
                    MessageBox.Show("Erreur SQL : " + ex.Message);
            }
        }

        // =====================================================
        // ✏️ UPDATE
        // =====================================================
        public void UpdateMarque(Marque m)
        {
            if (m == null) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"UPDATE Marque
                        SET Nom = @Nom,
                            Description = @Description,
                            DateModification = GETDATE()
                        WHERE Id = @Id AND Version = @Version";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Nom", m.Nom.Trim());
                    cmd.Parameters.AddWithValue("@Description",
                        string.IsNullOrWhiteSpace(m.Description)
                            ? (object)DBNull.Value
                            : m.Description.Trim());
                    cmd.Parameters.AddWithValue("@Id", m.Id);
                    cmd.Parameters.AddWithValue("@Version", m.Version);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                        MessageBox.Show("⚠️ Données modifiées par un autre utilisateur !");
                }
                LoadMarques();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    MessageBox.Show("⚠️ Une marque avec ce nom existe déjà !");
                else
                    MessageBox.Show("Erreur SQL : " + ex.Message);
            }
        }

        // =====================================================
        // ❌ DELETE
        // =====================================================
        public void DeleteMarque(Marque m)
        {
            if (m == null) return;

            var result = MessageBox.Show(
                $"Supprimer la marque '{m.Nom}' ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Marque WHERE Id = @Id AND Version = @Version";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", m.Id);
                    cmd.Parameters.AddWithValue("@Version", m.Version);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                        MessageBox.Show("⚠️ Données modifiées par un autre utilisateur. Veuillez rafraîchir.");
                }
                LoadMarques();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Erreur SQL : " + ex.Message);
            }
        }
    }
}




//using System;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;
//using System.Windows;
//using Microsoft.Data.SqlClient;
//using Stock741.Model;

//namespace Stock741
//{
//    public class MarqueViewModel : INotifyPropertyChanged
//    {
//        // =====================================================
//        // 🔔 INOTIFYPROPERTYCHANGED
//        // =====================================================
//        public event PropertyChangedEventHandler PropertyChanged;

//        protected void OnPropertyChanged([CallerMemberName] string p = null)
//            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

//        // =====================================================
//        // ⚙️ CONNEXION
//        // =====================================================
//        private readonly string _connectionString =
//            "Server=ULYSSESHOP\\SQLEXPRESS;Database=Stock741;User Id=Stock741User;Password=123;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

//        // =====================================================
//        // 📋 PROPRIÉTÉS
//        // =====================================================
//        public ObservableCollection<Marque> Marques { get; set; }
//            = new ObservableCollection<Marque>();

//        private Marque _selectedMarque;
//        public Marque SelectedMarque
//        {
//            get => _selectedMarque;
//            set
//            {
//                _selectedMarque = value;
//                OnPropertyChanged();
//                OnPropertyChanged(nameof(AMarqueSelectionnee));
//            }
//        }

//        public bool AMarqueSelectionnee => SelectedMarque != null;

//        // =====================================================
//        // 🔄 LOAD
//        // =====================================================
//        public void LoadMarques()
//        {
//            Marques.Clear();

//            using (SqlConnection conn = new SqlConnection(_connectionString))
//            {
//                conn.Open();

//                string query = "SELECT Id, Nom, Description, DateCreation, DateModification, Actif, Version FROM Marque ORDER BY Nom";
//                SqlCommand cmd = new SqlCommand(query, conn);
//                SqlDataReader reader = cmd.ExecuteReader();

//                while (reader.Read())
//                {
//                    Marques.Add(new Marque
//                    {
//                        Id = reader.GetInt32(0),
//                        Nom = reader.GetString(1),
//                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
//                        DateCreation = reader.GetDateTime(3),
//                        DateModification = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
//                        Actif = reader.GetBoolean(5),
//                        Version = (byte[])reader[6]
//                    });
//                }
//            }
//        }

//        // =====================================================
//        // ➕ ADD
//        // =====================================================
//        public void AddMarque(Marque m)
//        {
//            if (m == null || string.IsNullOrWhiteSpace(m.Nom))
//            {
//                MessageBox.Show("⚠️ Le nom est obligatoire !");
//                return;
//            }

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(_connectionString))
//                {
//                    conn.Open();
//                    string query = "INSERT INTO Marque (Nom, Description) VALUES (@Nom, @Description)";
//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    cmd.Parameters.AddWithValue("@Nom", m.Nom.Trim());
//                    cmd.Parameters.AddWithValue("@Description",
//                        string.IsNullOrWhiteSpace(m.Description)
//                            ? (object)DBNull.Value
//                            : m.Description.Trim());
//                    cmd.ExecuteNonQuery();
//                }
//                LoadMarques();
//            }
//            catch (SqlException ex)
//            {
//                if (ex.Number == 2627 || ex.Number == 2601)
//                    MessageBox.Show("⚠️ Cette marque existe déjà !");
//                else
//                    MessageBox.Show("Erreur SQL : " + ex.Message);
//            }
//        }

//        // =====================================================
//        // ✏️ UPDATE
//        // =====================================================
//        public void UpdateMarque(Marque m)
//        {
//            if (m == null) return;

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(_connectionString))
//                {
//                    conn.Open();
//                    string query = @"UPDATE Marque
//                        SET Nom = @Nom,
//                            Description = @Description,
//                            DateModification = GETDATE()
//                        WHERE Id = @Id AND Version = @Version";
//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    cmd.Parameters.AddWithValue("@Nom", m.Nom.Trim());
//                    cmd.Parameters.AddWithValue("@Description",
//                        string.IsNullOrWhiteSpace(m.Description)
//                            ? (object)DBNull.Value
//                            : m.Description.Trim());
//                    cmd.Parameters.AddWithValue("@Id", m.Id);
//                    cmd.Parameters.AddWithValue("@Version", m.Version);

//                    int rows = cmd.ExecuteNonQuery();
//                    if (rows == 0)
//                        MessageBox.Show("⚠️ Données modifiées par un autre utilisateur !");
//                }
//                LoadMarques();
//            }
//            catch (SqlException ex)
//            {
//                if (ex.Number == 2627 || ex.Number == 2601)
//                    MessageBox.Show("⚠️ Une marque avec ce nom existe déjà !");
//                else
//                    MessageBox.Show("Erreur SQL : " + ex.Message);
//            }
//        }

//        // =====================================================
//        // ❌ DELETE
//        // =====================================================
//        public void DeleteMarque(Marque m)
//        {
//            if (m == null) return;

//            var result = MessageBox.Show(
//                $"Supprimer la marque '{m.Nom}' ?",
//                "Confirmation",
//                MessageBoxButton.YesNo,
//                MessageBoxImage.Warning);

//            if (result != MessageBoxResult.Yes) return;

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(_connectionString))
//                {
//                    conn.Open();
//                    string query = "DELETE FROM Marque WHERE Id = @Id AND Version = @Version";
//                    SqlCommand cmd = new SqlCommand(query, conn);
//                    cmd.Parameters.AddWithValue("@Id", m.Id);
//                    cmd.Parameters.AddWithValue("@Version", m.Version);

//                    int rows = cmd.ExecuteNonQuery();
//                    if (rows == 0)
//                        MessageBox.Show("⚠️ Données modifiées par un autre utilisateur. Veuillez rafraîchir.");
//                }
//                LoadMarques();
//            }
//            catch (SqlException ex)
//            {
//                MessageBox.Show("Erreur SQL : " + ex.Message);
//            }
//        }
//    }
//}







//using System;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Windows;
//using Microsoft.Data.SqlClient;
//using Stock741.Model;

//namespace Stock741
//{
//    public class MarqueViewModel
//    {
//        private string connectionString = "Server=ULYSSESHOP\\SQLEXPRESS;Database=Stock741;User Id=Stock741User;Password=123;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

//        public ObservableCollection<Marque> Marques { get; set; } = new ObservableCollection<Marque>();

//        public Marque SelectedMarque { get; set; }

//        // =========================
//        // 🔄 LOAD
//        // =========================
//        public void LoadMarques()
//        {
//            Marques.Clear();

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                conn.Open();

//                string query = "SELECT Id, Nom, Description, DateCreation, DateModification, Actif, Version FROM Marque";
//                SqlCommand cmd = new SqlCommand(query, conn);

//                SqlDataReader reader = cmd.ExecuteReader();

//                while (reader.Read())
//                {
//                    Marques.Add(new Marque
//                    {
//                        Id = reader.GetInt32(0),
//                        Nom = reader.GetString(1),
//                        Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
//                        DateCreation = reader.GetDateTime(3),
//                        DateModification = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
//                        Actif = reader.GetBoolean(5),
//                        Version = (byte[])reader[6]
//                    });
//                }
//            }
//        }

//        // =========================
//        // ➕ ADD
//        // =========================
//        public void AddMarque(Marque m)
//        {
//            if (m == null || string.IsNullOrWhiteSpace(m.Nom))
//            {
//                MessageBox.Show("⚠️ Le nom est obligatoire !");
//                return;
//            }

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    string query = "INSERT INTO Marque (Nom, Description) VALUES (@Nom, @Description)";
//                    SqlCommand cmd = new SqlCommand(query, conn);

//                    cmd.Parameters.AddWithValue("@Nom", m.Nom);
//                    cmd.Parameters.AddWithValue("@Description", m.Description ?? "");

//                    cmd.ExecuteNonQuery();
//                }

//                LoadMarques();
//            }
//            catch (SqlException ex)
//            {
//                if (ex.Number == 2627 || ex.Number == 2601)
//                {
//                    MessageBox.Show("⚠️ Cette marque existe déjà !");
//                }
//                else
//                {
//                    MessageBox.Show("Erreur SQL : " + ex.Message);
//                }
//            }
//        }

//        // =========================
//        // ✏️ UPDATE
//        // =========================
//        public void UpdateMarque(Marque m)
//        {
//            if (m == null)
//                return;

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    string query = @"
//                        UPDATE Marque
//                        SET Nom = @Nom,
//                            Description = @Description,
//                            DateModification = GETDATE()
//                        WHERE Id = @Id AND Version = @Version";

//                    SqlCommand cmd = new SqlCommand(query, conn);

//                    cmd.Parameters.AddWithValue("@Nom", m.Nom);
//                    cmd.Parameters.AddWithValue("@Description", m.Description ?? "");
//                    cmd.Parameters.AddWithValue("@Id", m.Id);
//                    cmd.Parameters.AddWithValue("@Version", m.Version);

//                    int rows = cmd.ExecuteNonQuery();

//                    if (rows == 0)
//                    {
//                        MessageBox.Show("⚠️ Données modifiées par un autre utilisateur !");
//                    }
//                }

//                LoadMarques();
//            }
//            catch (SqlException ex)
//            {
//                if (ex.Number == 2627 || ex.Number == 2601)
//                {
//                    MessageBox.Show("⚠️ Une marque avec ce nom existe déjà !");
//                }
//                else
//                {
//                    MessageBox.Show("Erreur SQL : " + ex.Message);
//                }
//            }
//        }

//        // =========================
//        // ❌ DELETE
//        // =========================
//        public void DeleteMarque(Marque m)
//        {
//            if (m == null)
//                return;

//            var result = MessageBox.Show(
//                $"Supprimer la marque '{m.Nom}' ?",
//                "Confirmation",
//                MessageBoxButton.YesNo,
//                MessageBoxImage.Warning);

//            if (result != MessageBoxResult.Yes)
//                return;

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                conn.Open();

//                string query = "DELETE FROM Marque WHERE Id = @Id";
//                SqlCommand cmd = new SqlCommand(query, conn);

//                cmd.Parameters.AddWithValue("@Id", m.Id);

//                cmd.ExecuteNonQuery();
//            }

//            LoadMarques();
//        }
//    }
//}



//using System;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Windows;
//using Microsoft.Data.SqlClient;
//using Stock741.Model;

//namespace Stock741
//{
//    public class MarqueViewModel
//    {      
//        private string connectionString =  "Server=ULYSSESHOP\\SQLEXPRESS;Database=Stock741;User Id=Stock741User;Password=123;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

//        public ObservableCollection<Marque> Marques { get; set; } = new ObservableCollection<Marque>();

//        public Marque SelectedMarque { get; set; }

//        // =========================
//        // 🔄 LOAD
//        // =========================
//        public void LoadMarques()
//        {
//            Marques.Clear();

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                conn.Open();

//                string query = "SELECT Id, Nom, Description, DateCreation, DateModification, Actif, Version FROM Marque";
//                SqlCommand cmd = new SqlCommand(query, conn);

//                SqlDataReader reader = cmd.ExecuteReader();

//                while (reader.Read())
//                {
//                    Marques.Add(new Marque
//                    {
//                        Id = reader.GetInt32(0),
//                        Nom = reader.GetString(1),
//                        Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
//                        DateCreation = reader.GetDateTime(3),
//                        DateModification = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
//                        Actif = reader.GetBoolean(5),
//                        Version = (byte[])reader[6]
//                    });
//                }
//            }
//        }

//        // =========================
//        // ➕ ADD
//        // =========================
//        public void AddMarque(Marque m)
//        {
//            if (m == null || string.IsNullOrWhiteSpace(m.Nom))
//            {
//                MessageBox.Show("⚠️ Le nom est obligatoire !");
//                return;
//            }

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    string query = "INSERT INTO Marque (Nom, Description) VALUES (@Nom, @Description)";
//                    SqlCommand cmd = new SqlCommand(query, conn);

//                    cmd.Parameters.AddWithValue("@Nom", m.Nom);
//                    cmd.Parameters.AddWithValue("@Description", m.Description ?? "");

//                    cmd.ExecuteNonQuery();
//                }

//                LoadMarques();
//            }
//            catch (SqlException ex)
//            {
//                if (ex.Number == 2627 || ex.Number == 2601)
//                {
//                    MessageBox.Show("⚠️ Cette marque existe déjà !");
//                }
//                else
//                {
//                    MessageBox.Show("Erreur SQL : " + ex.Message);
//                }
//            }
//        }

//        // =========================
//        // ✏️ UPDATE
//        // =========================
//        public void UpdateMarque(Marque m)
//        {
//            if (m == null)
//                return;

//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();

//                    string query = @"
//                        UPDATE Marque
//                        SET Nom = @Nom,
//                            Description = @Description,
//                            DateModification = GETDATE()
//                        WHERE Id = @Id AND Version = @Version";

//                    SqlCommand cmd = new SqlCommand(query, conn);

//                    cmd.Parameters.AddWithValue("@Nom", m.Nom);
//                    cmd.Parameters.AddWithValue("@Description", m.Description ?? "");
//                    cmd.Parameters.AddWithValue("@Id", m.Id);
//                    cmd.Parameters.AddWithValue("@Version", m.Version);

//                    int rows = cmd.ExecuteNonQuery();

//                    if (rows == 0)
//                    {
//                        MessageBox.Show("⚠️ Données modifiées par un autre utilisateur !");
//                    }
//                }

//                LoadMarques();
//            }
//            catch (SqlException ex)
//            {
//                if (ex.Number == 2627 || ex.Number == 2601)
//                {
//                    MessageBox.Show("⚠️ Une marque avec ce nom existe déjà !");
//                }
//                else
//                {
//                    MessageBox.Show("Erreur SQL : " + ex.Message);
//                }
//            }
//        }

//        // =========================
//        // ❌ DELETE
//        // =========================
//        public void DeleteMarque(Marque m)
//        {
//            if (m == null)
//                return;

//            var result = MessageBox.Show(
//                $"Supprimer la marque '{m.Nom}' ?",
//                "Confirmation",
//                MessageBoxButton.YesNo,
//                MessageBoxImage.Warning);

//            if (result != MessageBoxResult.Yes)
//                return;

//            using (SqlConnection conn = new SqlConnection(connectionString))
//            {
//                conn.Open();

//                string query = "DELETE FROM Marque WHERE Id = @Id";
//                SqlCommand cmd = new SqlCommand(query, conn);

//                cmd.Parameters.AddWithValue("@Id", m.Id);

//                cmd.ExecuteNonQuery();
//            }

//            LoadMarques();
//        }
//    }
//}




//using System.Collections.ObjectModel;
//using Microsoft.Data.SqlClient;
//using System.Windows;

//public class MarqueViewModel
//{
//    //private string connectionString = "Server=ULYSSESHOP\\SQLEXPRESS;Database=Stock741;User Id=Stock741User;Password=TIP_971;";
//    //private string connectionString =     "Server=ULYSSESHOP\\SQLEXPRESS;Database=Stock741;Integrated Security=True;";

//    private string connectionString =  "Server=ULYSSESHOP\\SQLEXPRESS;Database=Stock741;User Id=Stock741User;Password=TIP_971;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

//    public Marque SelectedMarque { get; set; }

//    public ObservableCollection<Marque> Marques { get; set; } = new ObservableCollection<Marque>();

//    // Charger toutes les marques
//    public void LoadMarques()
//    {
//        Marques.Clear();
//        using (SqlConnection conn = new SqlConnection(connectionString))
//        {
//            conn.Open();
//            string query = "SELECT Id, Nom, Description, DateCreation, DateModification, Actif, Version FROM Marque";
//            SqlCommand cmd = new SqlCommand(query, conn);
//            SqlDataReader reader = cmd.ExecuteReader();
//            while (reader.Read())
//            {
//                Marques.Add(new Marque
//                {
//                    Id = reader.GetInt32(0),
//                    Nom = reader.GetString(1),
//                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
//                    DateCreation = reader.GetDateTime(3),
//                    DateModification = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
//                    Actif = reader.GetBoolean(5),
//                    Version = (byte[])reader[6]
//                });
//            }
//        }
//    }

//    // Ajouter une marque
//    public void AddMarque(Marque m)
//    {
//        using (SqlConnection conn = new SqlConnection(connectionString))
//        {
//            conn.Open();
//            string query = "INSERT INTO Marque (Nom, Description) VALUES (@Nom, @Description)";
//            SqlCommand cmd = new SqlCommand(query, conn);
//            cmd.Parameters.AddWithValue("@Nom", m.Nom);
//            cmd.Parameters.AddWithValue("@Description", m.Description ?? "");
//            cmd.ExecuteNonQuery();
//        }
//        LoadMarques();
//    }

//    // Modifier une marque avec ROWVERSION pour gérer la concurrence
//    public void UpdateMarque(Marque m)
//    {
//        using (SqlConnection conn = new SqlConnection(connectionString))
//        {
//            conn.Open();
//            string query = @"
//                UPDATE Marque
//                SET Nom = @Nom, Description = @Description, DateModification = GETDATE()
//                WHERE Id = @Id AND Version = @Version";
//            SqlCommand cmd = new SqlCommand(query, conn);
//            cmd.Parameters.AddWithValue("@Nom", m.Nom);
//            cmd.Parameters.AddWithValue("@Description", m.Description ?? "");
//            cmd.Parameters.AddWithValue("@Id", m.Id);
//            cmd.Parameters.AddWithValue("@Version", m.Version);
//            int rows = cmd.ExecuteNonQuery();
//            if (rows == 0)
//                MessageBox.Show("Cette marque a été modifiée par un autre utilisateur !");
//        }
//        LoadMarques();
//    }

//    // Supprimer une marque
//    public void DeleteMarque(Marque m)
//    {
//        using (SqlConnection conn = new SqlConnection(connectionString))
//        {
//            conn.Open();
//            string query = "DELETE FROM Marque WHERE Id = @Id";
//            SqlCommand cmd = new SqlCommand(query, conn);
//            cmd.Parameters.AddWithValue("@Id", m.Id);
//            cmd.ExecuteNonQuery();
//        }
//        LoadMarques();
//    }
//}