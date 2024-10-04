using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Dusza_Fogadas.pages
{
    public partial class UjJatek : Window
    {
        private List<string> alanyok = new List<string>();
        private List<string> esemenyek = new List<string>();

        public UjJatek()
        {
            InitializeComponent();
            InitializeLabels();
        }

        private void InitializeLabels()
        {
            Label alanyCim = new Label { Content = "Alanyok", Style = FindResource("ListTitle") as Style };
            spAlanyok.Children.Add(alanyCim);
            spAlanyok.Children.Add(new Separator { Style = FindResource("Separator") as Style });

            Label esemenyCim = new Label { Content = "Események", Style = FindResource("ListTitle") as Style };
            spEsemenyek.Children.Add(esemenyCim);
            spEsemenyek.Children.Add(new Separator { Style = FindResource("Separator") as Style });
        }

        private void btnFelveszAlany_Click(object sender, RoutedEventArgs e)
        {
            AddToList(tbAlany, alanyok, spAlanyok);
        }

        private void btnTorolAlany(object sender, RoutedEventArgs e)
        {
            RemoveFromList(sender, alanyok, spAlanyok);
        }

        private void btnFelveszEsemeny_Click(object sender, RoutedEventArgs e)
        {
            AddToList(tbEsemeny, esemenyek, spEsemenyek);
        }

        private void btnTorolEsemeny(object sender, RoutedEventArgs e)
        {
            RemoveFromList(sender, esemenyek, spEsemenyek);
        }

        private void btnMegse_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnLetrehoz_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UserSession.Instance.Id))
            {
                MessageBox.Show("Organizer ID is not set. Please log in again.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(tbNeve.Text) && !string.IsNullOrWhiteSpace(tbSzervezo.Text) && alanyok.Count > 0 && esemenyek.Count > 0)
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
                                                      "VALUES (@organizerId, @gameName, @numSubjects, @numEvents, 0, CURDATE(), NULL);";
                            int gameId;

                            using (MySqlCommand cmd = new MySqlCommand(insertGameQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@organizerId", UserSession.Instance.Id);
                                cmd.Parameters.AddWithValue("@gameName", tbNeve.Text);
                                cmd.Parameters.AddWithValue("@numSubjects", alanyok.Count);
                                cmd.Parameters.AddWithValue("@numEvents", esemenyek.Count);

                                cmd.ExecuteNonQuery();
                                gameId = (int)cmd.LastInsertedId; // Get the newly created game ID
                            }

                            // Insert subjects and events
                            InsertSubjects(conn, gameId);
                            InsertEvents(conn, gameId);

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

        private void AddToList(TextBox textBox, List<string> list, StackPanel stackPanel)
        {
            if (!string.IsNullOrWhiteSpace(textBox.Text) && !list.Contains(textBox.Text))
            {
                Button itemButton = new Button { Content = "  " + textBox.Text, Style = FindResource("ListItem") as Style };
                itemButton.Click += (s, e) => RemoveFromList(itemButton, list, stackPanel);
                list.Add(textBox.Text);
                stackPanel.Children.Add(itemButton);
                stackPanel.Children.Add(new Separator { Style = FindResource("Separator") as Style });
                textBox.Clear();
            }
            else
            {
                MessageBox.Show("Ez az alany vagy esemény már hozzá lett adva!");
            }
        }

        private void RemoveFromList(object sender, List<string> list, StackPanel stackPanel)
        {
            Button button = sender as Button;
            list.Remove(button.Content.ToString());
            int index = stackPanel.Children.IndexOf(button);
            stackPanel.Children.RemoveAt(index); // Remove the button
            stackPanel.Children.RemoveAt(index); // Remove the separator
        }

        private void InsertSubjects(MySqlConnection conn, int gameId)
        {
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
        }

        private void InsertEvents(MySqlConnection conn, int gameId)
        {
            foreach (string eventName in esemenyek)
            {
                string insertEventQuery = "INSERT INTO events (game_id, description) VALUES (@gameId, @eventDescription);";

                using (MySqlCommand cmd = new MySqlCommand(insertEventQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@gameId", gameId);
                    cmd.Parameters.AddWithValue("@eventDescription", eventName);
                    cmd.ExecuteNonQuery();
                }
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
            }

            return gameNames;
        }
    }
}
