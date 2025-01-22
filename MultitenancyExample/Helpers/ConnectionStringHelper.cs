namespace MultitenancyExample.Helpers;

public class ConnectionStringHelper
{
    // This method is used to create the connection string
    // Right now it is set up for Sqlite3, but you can adjust it to your needs.
    public static string GetConnectionString(
            string databaseName
        ) =>
            $"Data Source=Databases/{databaseName};";
}