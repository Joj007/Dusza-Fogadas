using System.Collections.ObjectModel;

public class Game
{
    public int Id { get; set; }
    public string GameName { get; set; }
    public DateTime StartDate { get; set; }

    public ObservableCollection<Subject> Subjects { get; set; } = new ObservableCollection<Subject>();
    public ObservableCollection<Event> Events { get; set; } = new ObservableCollection<Event>();
}

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Event
{
    public int Id { get; set; }
    public string Description { get; set; }
}