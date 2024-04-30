namespace Kanafka.Postgres;

public class ProviderConfiguration
{
    public string ConnectionString { get; set; }

    public ProviderConfiguration(string connectionString)
    {
        ConnectionString = connectionString;
    }
}