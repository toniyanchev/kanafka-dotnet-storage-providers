using Kanafka.Storage;

namespace Kanafka.Postgres;

public class FailedMessagePersister : IFailedMessagePersister
{
    public async Task PersistAsync(FailedMessage failedMessage)
    {
        Console.WriteLine("Save failure in db :)");
    }
}