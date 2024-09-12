namespace Confluent.Kafka.Core.Hosting.Internal
{
    internal enum ExecutionResult
    {
        NoAvailableSlots,
        NoAvailableMessages,
        Dispatched,
        UnhandledException
    }
}
