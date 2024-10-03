using MySql.Data.MySqlClient;

public class UserSession
{
    private static UserSession _instance;
    public static UserSession Instance => _instance ??= new UserSession();

    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public decimal? Balance { get; set; }

    private string connectionString = "Server=localhost;Database=dusza-fogadas;Uid=root;Pwd=;";

    public string GetUserEmail(string userName)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT email FROM users WHERE name = @userName";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@userName", userName);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
        }
    }

    public string GetUserRole(string userName)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            string query = "SELECT role FROM users WHERE name = @userName";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@userName", userName);
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
        }
    }

    public decimal? GetUserBalance(string userName)
    {
        if (GetUserRole(userName) == "fogadó")
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT balance FROM users WHERE name = @userName";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userName", userName);
                    object result = cmd.ExecuteScalar();
                    return result != null ? (decimal?)Convert.ToDecimal(result) : null;
                }
            }
        }
        return -1;
    }
}