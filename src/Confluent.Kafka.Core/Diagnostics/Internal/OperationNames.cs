namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    /// <summary>
    /// Values for messaging.operation.name. Free-form, and also used to build the span name.
    /// </summary>
    internal static class OperationNames
    {
        public const string PublishOperation = "publish";
        public const string ReceiveOperation = "receive";
        public const string ProcessOperation = "process";
    }

    /// <summary>
    /// Values for messaging.operation.type. Unlike the operation name this is a closed set:
    /// https://github.com/open-telemetry/semantic-conventions/blob/v1.44.0/docs/messaging/messaging-spans.md
    /// Note that publishing maps to "send", not "publish".
    /// </summary>
    internal static class OperationTypes
    {
        public const string Send = "send";
        public const string Receive = "receive";
        public const string Process = "process";
    }
}
