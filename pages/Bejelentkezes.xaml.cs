using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Security.Cryptography;
using System.Text;
using System.Data;

namespace Dusza_Fogadas.pages
{
    public partial class Bejelentkezes : Window
    {
        public Bejelentkezes()
        {
            InitializeComponent();
        }

        private void btnBejelentkezik_Click(object sender, RoutedEventArgs e)
        {
            if (tbNeve.Text != "" && pbJelszo.Password != "")
            {
                string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string selectQuery = "SELECT id, email, role, balance, is_active FROM users WHERE name = @Name AND password_hash = @Password";
                        using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Name", tbNeve.Text);
                            cmd.Parameters.AddWithValue("@Password", HashPassword(pbJelszo.Password));

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    bool isActive = reader.GetBoolean("is_active");
                                    if (!isActive)
                                    {
                                        MessageBox.Show("A felhasználó profil nem aktív. További információért keressen fel egy adminisztrátort.");
                                        return;
                                    }

                                    UserSession.Instance.Id = reader["id"].ToString(); // Store user ID
                                    UserSession.Instance.UserName = tbNeve.Text;
                                    UserSession.Instance.Email = reader["email"].ToString();
                                    UserSession.Instance.Role = reader["role"].ToString();
                                    UserSession.Instance.Balance = reader.IsDBNull("balance") ? (decimal?)null : reader.GetDecimal("balance");

                                    MessageBox.Show("Bejelentkezés sikeres!");
                                    DialogResult = true;
                                    Close();
                                }
                                else
                                {
                                    MessageBox.Show("Hibás név vagy jelszó!");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hiba történt: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Töltsd ki az összes mezőt!");
            }
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
            Regisztracio regisztracio = new Regisztracio();
            regisztracio.Show();
        }
    }
}