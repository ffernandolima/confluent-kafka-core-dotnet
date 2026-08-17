using System;
using System.Diagnostics.Metrics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal abstract class MeterBase
    {
        protected Meter Meter { get; }
        protected string MeterName => Meter?.Name;
        protected string MeterVersion => Meter?.Version;

        protected MeterBase(string meterName, string meterVersion = null)
        {
            if (string.IsNullOrWhiteSpace(meterName))
            {
                throw new ArgumentException($"{nameof(meterName)} cannot be null or whitespace.", nameof(meterName));
            }

            Meter = new Meter(meterName, meterVersion);
        }
    }
}
