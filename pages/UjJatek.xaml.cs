using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dusza_Fogadas.pages
{
    /// <summary>
    /// Interaction logic for UjJatek.xaml
    /// </summary>
    public partial class UjJatek : Window
    {
        private List<string> alanyok = new List<string>();
        private List<string> esemenyek = new List<string>();
        public UjJatek()
        {
            InitializeComponent();
        }

        private void btnFelveszAlany_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbAlany.Text))
            {
         
                if (!alanyok.Contains(tbAlany.Text))
                {
                    Button alany = new Button();
                    alany.Content = tbAlany.Text;
                    alanyok.Add(tbAlany.Text);
                    spAlanyok.Children.Add(alany);
                    alany.Click += btnTorolAlany;
                    alany.Height = 20;

                 
                    tbAlany.Clear();
                }
                else
                {
                    MessageBox.Show("Ez az alany már hozzá lett adva!");
                }
            }
            else
            {
                MessageBox.Show("Adj meg alanyt!");
            }
        }

        private void btnTorolAlany(object sender, RoutedEventArgs e)
        {
            Button alany = sender as Button;
            alanyok.Remove(alany.Content.ToString());
            spAlanyok.Children.Remove(alany);
        }

        private void btnFelveszEsemeny_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbEsemeny.Text))
            {
               
                if (!esemenyek.Contains(tbEsemeny.Text))
                {
                    Button esemeny = new Button();
                    esemeny.Content = tbEsemeny.Text;
                    esemenyek.Add(tbEsemeny.Text);
                    spEsemenyek.Children.Add(esemeny);
                    esemeny.Click += btnTorolEsemeny;
                    esemeny.Height = 20;

                    
                    tbEsemeny.Clear();
                }
                else
                {
                    MessageBox.Show("Ez az esemény már hozzá lett adva!");
                }
            }
            else
            {
                MessageBox.Show("Adj meg eseményt!");
            }
        }

        private void btnTorolEsemeny(object sender, RoutedEventArgs e)
        {
            Button esemeny = sender as Button;
            esemenyek.Remove(esemeny.Content.ToString());
            spEsemenyek.Children.Remove(esemeny);
        }

        private void btnMegse_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnLetrehoz_Click(object sender, RoutedEventArgs e)
        {
           
            if (tbNeve.Text != "" && tbSzervezo.Text != "" && alanyok.Count() != 0 && esemenyek.Count() != 0)
            {
                List<string> aktivJatekok = GetAktivJatekok();

                if (!aktivJatekok.Contains(tbNeve.Text))
                {
                    string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();

                            
                            string insertGameQuery = "INSERT INTO games (organizer_id, game_name, num_subjects, num_events, is_closed, start_date, close_date) " +
                                                      "VALUES ((SELECT id FROM users WHERE name = @organizerName), @gameName, @numSubjects, @numEvents, 0, CURDATE(), NULL);";


                            using (MySqlCommand cmd = new MySqlCommand(insertGameQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@organizerName", tbSzervezo.Text);
                                cmd.Parameters.AddWithValue("@gameName", tbNeve.Text);
                                cmd.Parameters.AddWithValue("@numSubjects", alanyok.Count);
                                cmd.Parameters.AddWithValue("@numEvents", esemenyek.Count);

                                cmd.ExecuteNonQuery();
                            }

                           
                            int gameId;
                            using (MySqlCommand cmd = new MySqlCommand("SELECT LAST_INSERT_ID();", conn))
                            {
                                gameId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                           
                            foreach (string subject in alanyok)
                            {
                                string insertSubjectQuery = "INSERT INTO subjects (game_id, name) VALUES (@gameId, @subjectName);";

                                using (MySqlCommand cmd = new MySqlCommand(insertSubjectQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@gameId", gameId);
                                    cmd.Parameters.AddWithValue("@subjectName", subject);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            
                            foreach (string eventName in esemenyek)
                            {
                                string insertEventQuery = "INSERT INTO events (game_id, subject_id, description) VALUES (@gameId, NULL, @eventDescription);";

                                using (MySqlCommand cmd = new MySqlCommand(insertEventQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@gameId", gameId);
                                    cmd.Parameters.AddWithValue("@eventDescription", eventName);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            MessageBox.Show("Játék sikeresen létrehozva!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Hiba történt: {ex.Message}");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Ilyen névvel már létezik játék.");
                }
            }
            else
            {
                MessageBox.Show("Töltsd ki az összes mezőt!");
            }
        }


        static List<string> GetAktivJatekok()
        {
            List<string> gameNames = new List<string>();
            string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    
                    string query = "SELECT game_name FROM games WHERE is_closed = 0";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string gameName = reader.GetString("game_name");
                                gameNames.Add(gameName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }

            return gameNames;
        }
    }
}
