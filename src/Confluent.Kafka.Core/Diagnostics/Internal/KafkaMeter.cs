using System;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    /// <summary>
    /// Owns the messaging instruments. A single instance for the process: 
    /// instruments are cheap to keep and nearly free to record into when no MeterProvider is listening, 
    /// so there is nothing to scope per client.
    /// </summary>
    internal sealed class KafkaMeter : MeterBase
    {
        private static readonly AssemblyName AssemblyName = typeof(KafkaMeter).Assembly.GetName();

        private static readonly Lazy<KafkaMeter> Factory = new(
            () => new KafkaMeter(), isThreadSafe: true);

        public static KafkaMeter Instance => Factory.Value;

        public Histogram<double> ClientOperationDuration { get; }
        public Histogram<double> ProcessDuration { get; }
        public Counter<long> ClientSentMessages { get; }
        public Counter<long> ClientConsumedMessages { get; }

        private KafkaMeter()
            : base(AssemblyName.Name, AssemblyName.Version!.ToString())
        {
            ClientOperationDuration = Meter.CreateHistogram(
                SemanticConventions.Metrics.ClientOperationDuration,
                SemanticConventions.Metrics.DurationUnit,
                "Duration of messaging operation initiated by a producer or consumer client.",
                tags: null,
                advice: new InstrumentAdvice<double>
                {
                    HistogramBucketBoundaries = SemanticConventions.Metrics.DurationBuckets
                });

            ProcessDuration = Meter.CreateHistogram(
                SemanticConventions.Metrics.ProcessDuration,
                SemanticConventions.Metrics.DurationUnit,
                "Duration of processing operation.",
                tags: null,
                advice: new InstrumentAdvice<double>
                {
                    HistogramBucketBoundaries = SemanticConventions.Metrics.DurationBuckets
                });

            ClientSentMessages = Meter.CreateCounter<long>(
                SemanticConventions.Metrics.ClientSentMessages,
                SemanticConventions.Metrics.MessageUnit,
                "Number of messages producer attempted to send to the broker.");

            ClientConsumedMessages = Meter.CreateCounter<long>(
                SemanticConventions.Metrics.ClientConsumedMessages,
                SemanticConventions.Metrics.MessageUnit,
                "Number of messages that were delivered to the application.");
        }
    }
}
