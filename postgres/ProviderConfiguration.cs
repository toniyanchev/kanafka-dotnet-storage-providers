namespace Kanafka.Postgres;

/// <summary>
/// Configurations for Postgres Kanafka storage provider.
/// </summary>
public class ProviderConfiguration
{
    
    /// <summary>
    /// Connection string to your Postgres database.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// Create db schema and tables on application startup.
    /// </summary>
    public bool InstallDbObjectsOnStart { get; set; }

    public ProviderConfiguration(string connectionString)
    {
        ConnectionString = connectionString;
    }
}