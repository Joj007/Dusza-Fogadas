using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using MySql.Data.MySqlClient;

public class GameViewModel : INotifyPropertyChanged
{
    private Game _selectedGame;
    private Subject _selectedSubject;
    private Event _selectedEvent;

    public ObservableCollection<Game> Games { get; set; }
    public Game SelectedGame
    {
        get { return _selectedGame; }
        set
        {
            _selectedGame = value;
            OnPropertyChanged(nameof(SelectedGame));
            LoadDetailsForSelectedGame();
        }
    }

    public Subject SelectedSubject
    {
        get { return _selectedSubject; }
        set
        {
            _selectedSubject = value;
            OnPropertyChanged(nameof(SelectedSubject));
        }
    }

    public Event SelectedEvent
    {
        get { return _selectedEvent; }
        set
        {
            _selectedEvent = value;
            OnPropertyChanged(nameof(SelectedEvent));
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
            string query = "SELECT * FROM games WHERE is_closed = 0";
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
                        Subjects = new ObservableCollection<Subject>(),
                        Events = new ObservableCollection<Event>()
                    });
                }
            }
        }
    }

    private void LoadDetailsForSelectedGame()
    {
        if (SelectedGame == null) return;

        string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";
        using (var connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Load subjects
            string subjectsQuery = "SELECT * FROM subjects WHERE game_id = @gameId";
            using (var command = new MySqlCommand(subjectsQuery, connection))
            {
                command.Parameters.AddWithValue("@gameId", SelectedGame.Id);
                using (var reader = command.ExecuteReader())
                {
                    SelectedGame.Subjects.Clear();
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

            // Load events
            string eventsQuery = "SELECT * FROM events WHERE game_id = @gameId";
            using (var command = new MySqlCommand(eventsQuery, connection))
            {
                command.Parameters.AddWithValue("@gameId", SelectedGame.Id);
                using (var reader = command.ExecuteReader())
                {
                    SelectedGame.Events.Clear();
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
