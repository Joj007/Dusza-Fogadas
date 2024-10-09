using System;
using System.Collections.Generic;
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
                var query = "SELECT id, name, email, role, is_active FROM users";
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
            // You can implement additional logic if needed when a user is selected
        }

        private void DeactivateButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersListBox.SelectedItem is User selectedUser)
            {
                using (var connection = new MySqlConnection(UserSession.Instance.ConnectionString))
                {
                    connection.Open();
                    string query = "UPDATE users SET is_active = 0 WHERE id = @userId";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", selectedUser.Id);
                        command.ExecuteNonQuery();
                    }
                }
                LoadUsers(); // Refresh the list
                MessageBox.Show("User deactivated successfully.");
            }
        }

        private void GamesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // You can implement additional logic if needed when a game is selected
        }

        private void DeleteGameButton_Click(object sender, RoutedEventArgs e) //TODO problémás 
        {
            if (GamesListBox.SelectedItem is Game selectedGame)
            {
                using (var connection = new MySqlConnection(UserSession.Instance.ConnectionString))
                {
                    connection.Open();

                    // Step 1: Delete bets associated with the game
                    string deleteBetsQuery = "DELETE FROM bets WHERE game_id = @gameId";
                    using (var deleteBetsCommand = new MySqlCommand(deleteBetsQuery, connection))
                    {
                        deleteBetsCommand.Parameters.AddWithValue("@gameId", selectedGame.Id);
                        deleteBetsCommand.ExecuteNonQuery();
                    }

                    // Step 2: Delete results associated with events linked to the game
                    string deleteResultsQuery = "DELETE FROM results WHERE game_id = @gameId";
                    using (var deleteResultsCommand = new MySqlCommand(deleteResultsQuery, connection))
                    {
                        deleteResultsCommand.Parameters.AddWithValue("@gameId", selectedGame.Id);
                        deleteResultsCommand.ExecuteNonQuery();
                    }

                    // Step 3: Delete events associated with the game
                    string deleteEventsQuery = "DELETE FROM events WHERE game_id = @gameId";
                    using (var deleteEventsCommand = new MySqlCommand(deleteEventsQuery, connection))
                    {
                        deleteEventsCommand.Parameters.AddWithValue("@gameId", selectedGame.Id);
                        deleteEventsCommand.ExecuteNonQuery();
                    }

                    // Step 4: Delete subjects associated with the game
                    string deleteSubjectsQuery = "DELETE FROM subjects WHERE game_id = @gameId";
                    using (var deleteSubjectsCommand = new MySqlCommand(deleteSubjectsQuery, connection))
                    {
                        deleteSubjectsCommand.Parameters.AddWithValue("@gameId", selectedGame.Id);
                        deleteSubjectsCommand.ExecuteNonQuery();
                    }

                    // Step 5: Delete the game itself
                    string deleteGameQuery = "DELETE FROM games WHERE id = @gameId";
                    using (var deleteGameCommand = new MySqlCommand(deleteGameQuery, connection))
                    {
                        deleteGameCommand.Parameters.AddWithValue("@gameId", selectedGame.Id);
                        deleteGameCommand.ExecuteNonQuery();
                    }
                }

                LoadGames(); // Refresh the list
                MessageBox.Show("Game deleted successfully.");
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
