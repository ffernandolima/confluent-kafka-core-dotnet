using Confluent.Kafka.Core.Internal;
using System;

namespace Confluent.Kafka.Core.Conversion.Internal
{
    internal static class OptionsConverterExtensions
    {
        internal static TOptions ToOptions<TOptions>(this object sourceObject)
        {
            if (sourceObject is not IOptionsConverter<TOptions> optionsConverter)
            {
                var optionsConverterType = typeof(IOptionsConverter<TOptions>).ExtractTypeName();

                throw new InvalidCastException($"{nameof(sourceObject)} should be of type '{optionsConverterType}'.");
            }

            var options = optionsConverter.ToOptions();

            return options;
        }
    }
}
