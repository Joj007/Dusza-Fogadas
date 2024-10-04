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
            string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Fetch games only for the logged-in user's organizer_id
                string query = "SELECT id, game_name, start_date FROM games WHERE is_closed = 0 AND organizer_id = @organizerId";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@organizerId", UserSession.Instance.Id);
                    using (var reader = command.ExecuteReader())
                    {
                        ActiveGames.Clear(); // Clear any existing games
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

            // Load subjects and events for the selected game
            string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Load subjects
                var subjects = new ObservableCollection<Subject>();
                string subjectsQuery = "SELECT * FROM subjects WHERE game_id = @gameId";
                using (var command = new MySqlCommand(subjectsQuery, connection))
                {
                    command.Parameters.AddWithValue("@gameId", game.Id);
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

                // Load events and create combinations
                var events = new ObservableCollection<Event>();
                string eventsQuery = "SELECT * FROM events WHERE game_id = @gameId";
                using (var command = new MySqlCommand(eventsQuery, connection))
                {
                    command.Parameters.AddWithValue("@gameId", game.Id);
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

                // Create combinations
                foreach (var subject in subjects)
                {
                    foreach (var ev in events)
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
            // Save results to the database
            string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                foreach (var combination in ResultCombinations)
                {
                    // Example: Save to the results table
                    string query = "INSERT INTO results (subject_name, event_description, result) VALUES (@subjectName, @eventDescription, @result)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@subjectName", combination.SubjectName);
                        command.Parameters.AddWithValue("@eventDescription", combination.EventDescription);
                        command.Parameters.AddWithValue("@result", combination.Result);
                        command.ExecuteNonQuery();
                    }
                }
            }

            MessageBox.Show("Results recorded successfully.");
        }
    }

    public class ResultCombination
    {
        public string SubjectName { get; set; }
        public string EventDescription { get; set; }
        public string Result { get; set; }
    }
}