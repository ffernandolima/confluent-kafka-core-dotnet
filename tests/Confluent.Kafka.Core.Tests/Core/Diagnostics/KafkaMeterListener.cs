using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;

namespace Confluent.Kafka.Core.Tests.Core.Diagnostics
{
    public sealed record MeasurementRecord(string InstrumentName, string Unit, double Value, IReadOnlyList<KeyValuePair<string, object>> Tags)
    {
        public object GetTag(string key) => Tags.FirstOrDefault(tag => tag.Key == key).Value;

        public bool HasTag(string key) => Tags.Any(tag => tag.Key == key);
    }

    public sealed class KafkaMeterListener : IDisposable
    {
        private readonly MeterListener _listener;
        private readonly List<MeasurementRecord> _measurements = [];
        private readonly object _sync = new();

        public IReadOnlyList<MeasurementRecord> Measurements
        {
            get
            {
                lock (_sync)
                {
                    return [.. _measurements];
                }
            }
        }

        public KafkaMeterListener()
        {
            _listener = new MeterListener
            {
                InstrumentPublished = (instrument, listener) =>
                {
                    if (instrument.Meter.Name == "Confluent.Kafka.Core")
                    {
                        listener.EnableMeasurementEvents(instrument);
                    }
                }
            };

            _listener.SetMeasurementEventCallback<double>((instrument, measurement, tags, _) => Record(instrument, measurement, tags));
            _listener.SetMeasurementEventCallback<long>((instrument, measurement, tags, _) => Record(instrument, measurement, tags));
            _listener.Start();
        }

        public IEnumerable<MeasurementRecord> ByInstrument(string instrumentName)
            => Measurements.Where(measurement => measurement.InstrumentName == instrumentName);

        private void Record<T>(Instrument instrument, T measurement, ReadOnlySpan<KeyValuePair<string, object>> tags)
            where T : struct
        {
            var record = new MeasurementRecord(
                instrument.Name,
                instrument.Unit,
                Convert.ToDouble(measurement),
                [.. tags.ToArray()]);

            lock (_sync)
            {
                _measurements.Add(record);
            }
        }

        public void Dispose() => _listener.Dispose();
    }
}
