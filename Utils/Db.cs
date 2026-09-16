using MySqlConnector;

namespace SmartAutoCashSystem.Utils;

public static class Db
{
    // Example connection string for XAMPP MySQL (root without password).
    // Adjust Pwd / host / port as needed.
    private const string ConnectionString = "Server=localhost;Database=SmartAutoCashDB;Uid=root;Pwd=;SslMode=None;";

    public static MySqlConnection CreateConnection() => new(ConnectionString);
}

