public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }

    public override string ToString()
    {
        return $"{Name} ({Email}) - ({Role}) {(IsActive ? "aktív" : "nem aktív")}";
    }
}