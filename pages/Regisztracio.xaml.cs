using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Dusza_Fogadas.pages
{
    public partial class Regisztracio : Window
    {
        public Regisztracio()
        {
            InitializeComponent();
            cbSzerepkor.SelectedItem = cbSzerepkor.Items.Cast<ComboBoxItem>().FirstOrDefault(item => ((ComboBoxItem)item).Content.ToString() == "fogadó");
        }

        private void btnRegisztral_Click(object sender, RoutedEventArgs e)
        {
            // Basic Input Validation
            if (string.IsNullOrWhiteSpace(tbNev.Text) ||
                string.IsNullOrWhiteSpace(tbEmail.Text) ||
                string.IsNullOrWhiteSpace(pbJelszo.Password))
            {
                MessageBox.Show("Kérjük, töltse ki az összes mezőt!");
                return;
            }

            // Email Validation
            if (!IsValidEmail(tbEmail.Text))
            {
                MessageBox.Show("Kérjük, adjon meg egy érvényes email címet!");
                return;
            }

            // Username and Email Existence Check
            if (IsUsernameExists(tbNev.Text))
            {
                MessageBox.Show("Ez a név már létezik. Kérjük, válasszon másikat.");
                return;
            }

            if (IsEmailExists(tbEmail.Text))
            {
                MessageBox.Show("Ez az email cím már létezik. Kérjük, válasszon másikat.");
                return;
            }

            // Password Validation
            if (!IsValidPassword(pbJelszo.Password))
            {
                MessageBox.Show("A jelszónak legalább 8 karakter hosszúnak kell lennie, és tartalmaznia kell kis- és nagybetűt!");
                return;
            }

            // Determine the balance based on the role
            string selectedRole = ((ComboBoxItem)cbSzerepkor.SelectedItem).Content.ToString();
            object initialBalance = selectedRole == "fogadó" ? (object)100 : DBNull.Value;

            // Proceed with registration
            string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string insertQuery = "INSERT INTO users (name, email, password_hash, balance, role) VALUES (@name, @Email, @password, @balance, @role)";
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", tbNev.Text);
                        cmd.Parameters.AddWithValue("@Email", tbEmail.Text);
                        cmd.Parameters.AddWithValue("@password", HashPassword(pbJelszo.Password));
                        cmd.Parameters.AddWithValue("@balance", initialBalance); // Set balance based on role
                        cmd.Parameters.AddWithValue("@role", selectedRole); // Add selected role

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Regisztráció sikeres!");
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt: {ex.Message}");
                }
            }
        }

        // Email validation method
        private bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.LastIndexOf('.') > email.IndexOf('@') + 1;
        }

        // Username existence check
        private bool IsUsernameExists(string username)
        {
            string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE name = @name";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", username);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt: {ex.Message}");
                    return false;
                }
            }
        }

        // Email existence check
        private bool IsEmailExists(string email)
        {
            string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE email = @Email";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt: {ex.Message}");
                    return false;
                }
            }
        }

        // Password validation method
        private bool IsValidPassword(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower);
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
