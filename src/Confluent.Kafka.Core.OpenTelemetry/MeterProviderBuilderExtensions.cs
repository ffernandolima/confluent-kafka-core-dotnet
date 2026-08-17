namespace OpenTelemetry.Metrics
{
    public static class MeterProviderBuilderExtensions
    {
        public static MeterProviderBuilder AddKafkaCoreInstrumentation(this MeterProviderBuilder builder)
            => builder.AddMeter("Confluent.Kafka.Core");
    }
}
