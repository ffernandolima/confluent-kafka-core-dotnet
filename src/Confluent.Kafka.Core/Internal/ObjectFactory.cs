using Microsoft.Extensions.DependencyInjection;
using System;

namespace Confluent.Kafka.Core.Internal
{
    internal static class ObjectFactory
    {
        public static object TryCreateInstance(IServiceProvider serviceProvider, Type objectType)
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
