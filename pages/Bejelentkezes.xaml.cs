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
            if (tbNeve.Text != "" && pbJelszo.Password != "")
            {
                string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string selectQuery = "SELECT * FROM users WHERE name = @Name AND password_hash = @Password";
                        using (MySqlCommand cmd = new MySqlCommand(selectQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@Name", tbNeve.Text);
                            cmd.Parameters.AddWithValue("@Password", HashPassword(pbJelszo.Password));

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    MessageBox.Show("Sikeres bejelentkezés!");

                                    string userName = tbNeve.Text;

                                    UserSession.Instance.UserName = userName;
                                    UserSession.Instance.Email = UserSession.Instance.GetUserEmail(userName);
                                    UserSession.Instance.Role = UserSession.Instance.GetUserRole(userName); 
                                    UserSession.Instance.Balance = UserSession.Instance.GetUserBalance(userName); 

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
