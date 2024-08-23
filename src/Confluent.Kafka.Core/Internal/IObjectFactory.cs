using System;

namespace Confluent.Kafka.Core.Internal
{
    internal interface IObjectFactory
    {
        object TryCreateInstance(IServiceProvider serviceProvider, Type objectType);
    }
}
