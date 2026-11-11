
using MySqlConnector;

namespace CyberSecurityAwarenessBotPart2;

public static class DatabaseConnection
{
    private static readonly string ConnectionString =
        "Server=127.0.0.1;" +
        "Port=3306;" +
        "Database=CyberSecurityBotDB;" +
        "User ID=root;" +
        "Password=Kaydee@25;";

    public static MySqlConnection GetConnection()
    {
        return new MySqlConnection(ConnectionString);
    }
}

