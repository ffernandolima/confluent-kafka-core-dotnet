using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Internal
{
    internal sealed class ObjectFactory
    {
        private static readonly Lazy<ObjectFactory> Factory = new(
         () => new ObjectFactory(), isThreadSafe: true);

        public static ObjectFactory Instance => Factory.Value;

        private ObjectFactory()
        { }

        public object TryCreateInstance(IServiceProvider serviceProvider, Type objectType)
        {
            if (objectType is null)
            {
                throw new ArgumentNullException(nameof(objectType), $"{nameof(objectType)} cannot be null.");
            }

            try
            {
                var instance = serviceProvider is null
                    ? Activator.CreateInstance(objectType)
                    : ActivatorUtilities.CreateInstance(serviceProvider, objectType);

                return instance;
            }
            catch
            {
                return null;
            }
        }
    }
}
