using System;
using System.Collections.ObjectModel;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Dusza_Fogadas
{
    public partial class AdminPanel : Window
    {
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Game> Games { get; set; }

        public AdminPanel()
        {
            InitializeComponent();
            Users = new ObservableCollection<User>();
            Games = new ObservableCollection<Game>();
            LoadUsers();
            LoadGames();
            UsersListBox.ItemsSource = Users;
            GamesListBox.ItemsSource = Games;
        }

        private void LoadUsers()
        {
            using (var connection = new MySqlConnection(UserSession.Instance.ConnectionString))
            {
                connection.Open();
                var query = "SELECT id, name, email, role, is_active FROM users WHERE role != 'admin'";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    Users.Clear();
                    while (reader.Read())
                    {
                        Users.Add(new User
                        {
                            Id = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            Email = reader.GetString("email"),
                            Role = reader.GetString("role"),
                            IsActive = reader.GetInt32("is_active") == 1 // Convert TINYINT to boolean
                        });
                    }
                }
            }
        }

        private void LoadGames()
        {
            using (var connection = new MySqlConnection(UserSession.Instance.ConnectionString))
            {
                connection.Open();
                string query = "SELECT id, game_name, start_date FROM games WHERE is_closed = 0";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    Games.Clear();
                    while (reader.Read())
                    {
                        var game = new Game
                        {
                            Id = reader.GetInt32("id"),
                            GameName = reader.GetString("game_name"),
                            StartDate = reader.GetDateTime("start_date"),
                            Subjects = LoadSubjectsForGame(reader.GetInt32("id")),
                            Events = LoadEventsForGame(reader.GetInt32("id"))
                        };

                        Games.Add(game);
                    }
                }
            }
        }

        private ObservableCollection<Subject> LoadSubjectsForGame(int gameId)
        {
            var subjects = new ObservableCollection<Subject>();

            using (var connection = new MySqlConnection(UserSession.Instance.ConnectionString))
            {
                connection.Open();
                string subjectsQuery = "SELECT id, name FROM subjects WHERE game_id = @gameId";
                using (var command = new MySqlCommand(subjectsQuery, connection))
                {
                    command.Parameters.AddWithValue("@gameId", gameId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subjects.Add(new Subject
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name")
                            });
                        }
                    }
                }
            }

            return subjects;
        }

        private ObservableCollection<Event> LoadEventsForGame(int gameId)
        {
            var events = new ObservableCollection<Event>();

            using (var connection = new MySqlConnection(UserSession.Instance.ConnectionString))
            {
                connection.Open();
                string eventsQuery = "SELECT id, description FROM events WHERE game_id = @gameId";
                using (var command = new MySqlCommand(eventsQuery, connection))
                {
                    command.Parameters.AddWithValue("@gameId", gameId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            events.Add(new Event
                            {
                                Id = reader.GetInt32("id"),
                                Description = reader.GetString("description")
                            });
                        }
                    }
                }
            }

            return events;
        }

        private void UsersListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedItem is User selectedUser)
            {
                if (selectedUser.IsActive)
                {
                    MessageBox.Show("A felhasználó aktív.");
                    return;
                }

                UpdateUserStatus(selectedUser.Id, true);
                LoadUsers(); // Refresh the list
                MessageBox.Show("Sikeresen aktiválva.");
            }
        }

        private void DeactivateButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedItem is User selectedUser)
            {
                if (!selectedUser.IsActive)
                {
                    MessageBox.Show("A felhasználó nem aktív.");
                    return;
                }

                UpdateUserStatus(selectedUser.Id, false);
                LoadUsers(); // Refresh the list
                MessageBox.Show("Sikeresen deaktiválva.");
            }
        }

        private void UpdateUserStatus(int userId, bool isActive)
        {
            using (var connection = new MySqlConnection(UserSession.Instance.ConnectionString))
            {
                connection.Open();
                string query = "UPDATE users SET is_active = @isActive WHERE id = @userId";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@isActive", isActive ? 1 : 0);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void GamesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void DeleteGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (GamesListBox.SelectedItem is Game selectedGame)
            {
                using (var connection = new MySqlConnection(UserSession.Instance.ConnectionString))
                {
                    connection.Open();
                    DeleteAssociatedRecords(selectedGame.Id, connection);
                    DeleteGame(selectedGame.Id, connection);
                }

                LoadGames(); // Refresh the list
                MessageBox.Show("Játék sikeresen törölve.");
            }
        }

        private void DeleteGame(int gameId, MySqlConnection connection)
        {
            string query = "DELETE FROM games WHERE id = @gameId";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@gameId", gameId);
                command.ExecuteNonQuery();
            }
        }
        private void DeleteAssociatedRecords(int gameId, MySqlConnection connection)
        {
            // Return bets to users before deleting the records
            ReturnBetsToUsers(gameId, connection);

            // Delete bets first
            string deleteBetsQuery = "DELETE FROM bets WHERE game_id = @gameId";
            using (var command = new MySqlCommand(deleteBetsQuery, connection))
            {
                command.Parameters.AddWithValue("@gameId", gameId);
                command.ExecuteNonQuery();
            }

            // Then delete events
            string deleteEventsQuery = "DELETE FROM events WHERE game_id = @gameId";
            using (var command = new MySqlCommand(deleteEventsQuery, connection))
            {
                command.Parameters.AddWithValue("@gameId", gameId);
                command.ExecuteNonQuery();
            }

            // Then delete results
            string deleteResultsQuery = "DELETE FROM results WHERE game_id = @gameId";
            using (var command = new MySqlCommand(deleteResultsQuery, connection))
            {
                command.Parameters.AddWithValue("@gameId", gameId);
                command.ExecuteNonQuery();
            }

            // Finally delete subjects
            string deleteSubjectsQuery = "DELETE FROM subjects WHERE game_id = @gameId";
            using (var command = new MySqlCommand(deleteSubjectsQuery, connection))
            {
                command.Parameters.AddWithValue("@gameId", gameId);
                command.ExecuteNonQuery();
            }
        }

        private void ReturnBetsToUsers(int gameId, MySqlConnection connection)
        {
            string query = "SELECT user_id, bet_amount FROM bets WHERE game_id = @gameId";
            List<(int userId, double amount)> bets = new List<(int, double)>();

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@gameId", gameId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int userId = reader.GetInt32("user_id");
                        double amount = reader.GetDouble("bet_amount");
                        bets.Add((userId, amount));
                    }
                }
            }

            foreach (var bet in bets)
            {
                ReturnBetToUser(bet.userId, bet.amount, connection);
            }
        }

        private void ReturnBetToUser(int userId, double amount, MySqlConnection connection)
        {
            string query = "UPDATE users SET balance = balance + @amount WHERE id = @userId";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@userId", userId);
                command.ExecuteNonQuery();
            }
        }

        private void btnVissza_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
