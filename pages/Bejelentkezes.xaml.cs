using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Security.Cryptography;
using System.Text;

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
            if (tbEmail.Text != "" && pbJelszo.Password != "")
            {
                string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string selectQuery = "SELECT * FROM users WHERE email = @Email AND password_hash = @Password";
                        using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Email", tbEmail.Text);
                            cmd.Parameters.AddWithValue("@Password", HashPassword(pbJelszo.Password));

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    MessageBox.Show("Sikeres bejelentkezés!");
                                    // Redirect to main application window

                                    string userName = tbUserName.Text; // Example //TODO ezt meg kell oldani

                                    UserSession.Instance.UserName = userName;
                                    UserSession.Instance.Email = UserSession.Instance.GetUserEmail(userName); // Retrieve email
                                    UserSession.Instance.Role = UserSession.Instance.GetUserRole(userName); // Retrieve role
                                    UserSession.Instance.Balance = UserSession.Instance.GetUserBalance(userName); // Retrieve balance

                                    MessageBox.Show("Bejelentkezés sikeres!");

                                    DialogResult = true;
                                    Close();
                                }
                                else
                                {
                                    MessageBox.Show("Hibás email vagy jelszó!");
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
                // Compute the hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a hex string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Convert to hex format
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
