namespace OpenTelemetry.Trace
{
    public static class TracerProviderBuilderExtensions
    {
        public static TracerProviderBuilder AddKafkaInstrumentation(this TracerProviderBuilder builder)
            => builder.AddSource("Confluent.Kafka.Core");
    }
}
