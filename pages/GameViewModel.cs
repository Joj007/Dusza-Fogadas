using System.Collections.ObjectModel;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;

public class GameViewModel : INotifyPropertyChanged
{
    private Game _selectedGame;

    public ObservableCollection<Game> Games { get; set; }
    public Game SelectedGame
    {
        get { return _selectedGame; }
        set
        {
            _selectedGame = value;
            OnPropertyChanged(nameof(SelectedGame));
            LoadDetailsForSelectedGame(); // Load subjects and events when a game is selected
        }
    }

    public GameViewModel()
    {
        Games = new ObservableCollection<Game>();
        LoadGames();
    }

    private void LoadGames()
    {
        string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM games WHERE is_closed = 0"; // Load only active games
            using (var command = new MySqlCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Games.Add(new Game
                    {
                        Id = reader.IsDBNull("id") ? 0 : reader.GetInt32("id"),
                        GameName = reader.IsDBNull("game_name") ? string.Empty : reader.GetString("game_name"),
                        StartDate = reader.IsDBNull("start_date") ? DateTime.MinValue : reader.GetDateTime("start_date"),
                        // Initialize subjects and events as empty lists
                        Subjects = new ObservableCollection<Subject>(),
                        Events = new ObservableCollection<Event>()
                    });
                }
            }
        }
    }

    private void LoadDetailsForSelectedGame()
    {
        if (SelectedGame == null)
            return;

        // Load subjects for the selected game
        string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string subjectsQuery = "SELECT * FROM subjects WHERE game_id = @gameId";
            using (var command = new MySqlCommand(subjectsQuery, connection))
            {
                command.Parameters.AddWithValue("@gameId", SelectedGame.Id);
                using (var reader = command.ExecuteReader())
                {
                    SelectedGame.Subjects.Clear(); // Clear existing subjects before adding
                    while (reader.Read())
                    {
                        SelectedGame.Subjects.Add(new Subject
                        {
                            Id = reader.IsDBNull("id") ? 0 : reader.GetInt32("id"),
                            Name = reader.IsDBNull("name") ? string.Empty : reader.GetString("name")
                        });
                    }
                }
            }

            // Load events for the selected game
            string eventsQuery = "SELECT * FROM events WHERE game_id = @gameId";
            using (var command = new MySqlCommand(eventsQuery, connection))
            {
                command.Parameters.AddWithValue("@gameId", SelectedGame.Id);
                using (var reader = command.ExecuteReader())
                {
                    SelectedGame.Events.Clear(); // Clear existing events before adding
                    while (reader.Read())
                    {
                        SelectedGame.Events.Add(new Event
                        {
                            Id = reader.IsDBNull("id") ? 0 : reader.GetInt32("id"),
                            Description = reader.IsDBNull("description") ? string.Empty : reader.GetString("description")
                        });
                    }
                }
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}