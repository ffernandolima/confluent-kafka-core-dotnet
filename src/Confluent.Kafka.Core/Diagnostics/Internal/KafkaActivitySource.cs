using System.Reflection;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class KafkaActivitySource : ActivitySourceBase
    {
        private static readonly AssemblyName AssemblyName = typeof(KafkaActivitySource).Assembly.GetName();

        public KafkaActivitySource()
            : base(AssemblyName.Name, AssemblyName.Version.ToString())
        { }
    }
}
