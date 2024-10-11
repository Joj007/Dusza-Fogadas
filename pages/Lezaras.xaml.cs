using System;
using System.Collections.ObjectModel;
using System.Windows;
using MySql.Data.MySqlClient;

namespace Dusza_Fogadas
{
    public partial class Lezaras : Window
    {
        public ObservableCollection<Game> ActiveGames { get; set; }
        public ObservableCollection<ResultCombination> ResultCombinations { get; set; }

        public Lezaras()
        {
            InitializeComponent();
            ActiveGames = new ObservableCollection<Game>();
            ResultCombinations = new ObservableCollection<ResultCombination>();
            LoadActiveGames();
            ActiveGamesListBox.ItemsSource = ActiveGames;
        }

        private void LoadActiveGames()
        {
            using (var connection = new MySqlConnection(UserSession.Instance.ConnectionString))
            {
                connection.Open();
                string query = "SELECT id, game_name, start_date FROM games WHERE is_closed = 0 AND organizer_id = @organizerId";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@organizerId", UserSession.Instance.Id);
                    using (var reader = command.ExecuteReader())
                    {
                        ActiveGames.Clear();
                        while (reader.Read())
                        {
                            ActiveGames.Add(new Game
                            {
                                Id = reader.GetInt32("id"),
                                GameName = reader.GetString("game_name"),
                                StartDate = reader.GetDateTime("start_date")
                            });
                        }
                    }
                }
            }
            ActiveGamesListBox.ItemsSource = ActiveGames;
        }

        private void ActiveGamesListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ActiveGamesListBox.SelectedItem is Game selectedGame)
            {
                LoadGameCombinations(selectedGame);
            }
        }

        private void LoadGameCombinations(Game game)
        {
            ResultCombinations.Clear();
            using (var connection = new MySqlConnection(UserSession.Instance.ConnectionString))
            {
                connection.Open();
                game.Subjects.Clear();
                game.Events.Clear();

                // Load subjects
                string subjectsQuery = "SELECT * FROM subjects WHERE game_id = @gameId";
                using (var command = new MySqlCommand(subjectsQuery, connection))
                {
                    command.Parameters.AddWithValue("@gameId", game.Id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            game.Subjects.Add(new Subject
                            {
                                Id = reader.GetInt32("id"),
                                Name = reader.GetString("name")
                            });
                        }
                    }
                }

                // Load events
                string eventsQuery = "SELECT * FROM events WHERE game_id = @gameId";
                using (var command = new MySqlCommand(eventsQuery, connection))
                {
                    command.Parameters.AddWithValue("@gameId", game.Id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            game.Events.Add(new Event
                            {
                                Id = reader.GetInt32("id"),
                                Description = reader.GetString("description")
                            });
                        }
                    }
                }

                // Create combinations
                foreach (var subject in game.Subjects)
                {
                    foreach (var ev in game.Events)
                    {
                        ResultCombinations.Add(new ResultCombination
                        {
                            SubjectName = subject.Name,
                            EventDescription = ev.Description,
                            Result = string.Empty // Initial result is empty
                        });
                    }
                }
            }
            ResultsItemsControl.ItemsSource = ResultCombinations;
        }

        private void RogzitButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedGame = ActiveGamesListBox.SelectedItem as Game;
            if (selectedGame == null)
            {
                MessageBox.Show("Nincsen játék kiválasztva.");
                return;
            }

            // Validate that all result fields are filled
            foreach (var combination in ResultCombinations)
            {
                if (string.IsNullOrWhiteSpace(combination.Result))
                {
                    MessageBox.Show("Kérlek töltsd ki az összes mezőt.");
                    return;
                }
            }

            using (var connection = new MySqlConnection(UserSession.Instance.ConnectionString))
            {
                connection.Open();
                foreach (var combination in ResultCombinations)
                {
                    int subjectId = GetSubjectIdByName(combination.SubjectName, selectedGame.Id, connection);
                    int eventId = GetEventIdByDescription(combination.EventDescription, selectedGame.Id, connection);
                    int numberOfBets = GetNumberOfBets(subjectId, eventId, connection);
                    double multiplier = numberOfBets > 0 ? 1 + 5 / Math.Pow(2, numberOfBets - 1) : 0;

                    string query = "INSERT INTO results (game_id, subject_id, event_id, actual_value, multiplier) VALUES (@gameId, @subjectId, @eventId, @result, @multiplier)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@gameId", selectedGame.Id);
                        command.Parameters.AddWithValue("@subjectId", subjectId);
                        command.Parameters.AddWithValue("@eventId", eventId);
                        command.Parameters.AddWithValue("@result", combination.Result);
                        command.Parameters.AddWithValue("@multiplier", multiplier);
                        command.ExecuteNonQuery();
                    }

                    ProcessBets(selectedGame.Id, subjectId, eventId, combination.Result, multiplier, connection);
                }
                UpdateGameStatus(selectedGame.Id, connection);
            }
            MessageBox.Show("Results recorded and game closed successfully.");
            Close();
        }

        private void ProcessBets(int gameId, int subjectId, int eventId, string actualValue, double multiplier, MySqlConnection connection)
        {
            string query = "SELECT user_id, bet_amount FROM bets WHERE game_id = @gameId AND subject_id = @subjectId AND event_id = @eventId AND bet_value = @actualValue";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@gameId", gameId);
                command.Parameters.AddWithValue("@subjectId", subjectId);
                command.Parameters.AddWithValue("@eventId", eventId);
                command.Parameters.AddWithValue("@actualValue", actualValue);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int userId = reader.GetInt32("user_id");
                        double betAmount = reader.GetDouble("bet_amount");
                        double winnings = betAmount * multiplier;

                        UpdateUserBalance(userId, winnings); // Ensure this command executes after the reader is closed
                    }
                } // Ensure the reader is closed here
            }
        }

        private void UpdateUserBalance(int userId, double winnings)
        {
            using (var connection = new MySqlConnection(UserSession.Instance.ConnectionString))
            {
                connection.Open();
                string query = "UPDATE users SET balance = balance + @winnings WHERE id = @userId";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@winnings", winnings);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdateGameStatus(int gameId, MySqlConnection connection)
        {
            string query = "UPDATE games SET is_closed = 1, close_date = @closeDate WHERE id = @gameId";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@closeDate", DateTime.Now);
                command.Parameters.AddWithValue("@gameId", gameId);
                command.ExecuteNonQuery();
            }
        }

        private int GetSubjectIdByName(string subjectName, int gameId, MySqlConnection connection)
        {
            string query = "SELECT id FROM subjects WHERE name = @subjectName AND game_id = @gameId";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@subjectName", subjectName);
                command.Parameters.AddWithValue("@gameId", gameId);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private int GetEventIdByDescription(string eventDescription, int gameId, MySqlConnection connection)
        {
            string query = "SELECT id FROM events WHERE description = @eventDescription AND game_id = @gameId";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@eventDescription", eventDescription);
                command.Parameters.AddWithValue("@gameId", gameId);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private int GetNumberOfBets(int subjectId, int eventId, MySqlConnection connection)
        {
            string query = "SELECT COUNT(*) FROM bets WHERE subject_id = @subjectId AND event_id = @eventId";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@subjectId", subjectId);
                command.Parameters.AddWithValue("@eventId", eventId);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class ResultCombination
    {
        public string SubjectName { get; set; }
        public string EventDescription { get; set; }
        public string Result { get; set; }
    }
}