namespace OpenTelemetry.Trace
{
    public static class TracerProviderBuilderExtensions
    {
        public static TracerProviderBuilder AddKafkaCoreInstrumentation(this TracerProviderBuilder builder)
            => builder.AddSource("Confluent.Kafka.Core");
    }
}
