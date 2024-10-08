namespace Confluent.Kafka.Core.Tests.Extensions
{
    public static class ObjectExtensions
    {
        public static TImplementation ToImplementation<TImplementation>(
            this object service)
        {
            var implementation = (TImplementation)service;

            return implementation;
        }
    }
}