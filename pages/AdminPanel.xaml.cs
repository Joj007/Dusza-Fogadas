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
                string query = "SELECT g.id, g.game_name, g.start_date FROM games g";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    Games.Clear();
                    while (reader.Read())
                    {
                        Games.Add(new Game
                        {
                            Id = reader.GetInt32("id"),
                            GameName = reader.GetString("game_name"),
                            StartDate = reader.GetDateTime("start_date")
                        });
                    }
                }
            }
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

        private void DeleteAssociatedRecords(int gameId, MySqlConnection connection)
        {
            string[] tables = { "bets", "results", "events", "subjects" };

            foreach (var table in tables)
            {
                string query = $"DELETE FROM {table} WHERE game_id = @gameId";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@gameId", gameId);
                    command.ExecuteNonQuery();
                }
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
    }
}
