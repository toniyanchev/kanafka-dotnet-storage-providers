using Kanafka.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Kanafka.Postgres;

public static class ServiceResolver
{
    public static void AddKanafkaPostgres(this IServiceCollection services)
    {
        services.AddKanafka();
        services.AddScoped<IFailedMessagePersister, FailedMessagePersister>();
    }
}