using Confluent.Kafka.Core.Internal;
using System;

namespace Confluent.Kafka.Core.Mapping
{
    internal static class MapperExtensions
    {
        internal static TDestination Map<TDestination>(this object sourceObject, params object[] args)
        {
            if (sourceObject is not IMapper<TDestination> mapper)
            {
                var mapperType = typeof(IMapper<TDestination>).ExtractTypeName();

                throw new InvalidCastException($"{nameof(sourceObject)} should be of type '{mapperType}'.");
            }

            var result = mapper.Map(args);

            return result;
        }
    }
}
