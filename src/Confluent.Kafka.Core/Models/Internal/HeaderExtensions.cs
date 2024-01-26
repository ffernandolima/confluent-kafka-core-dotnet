using System;

namespace Confluent.Kafka.Core.Models.Internal
{
    using System.Text;

    internal static class HeaderExtensions
    {
        public static string GetStringValue(this IHeader header, Encoding encoding)
        {
            if (header is null)
            {
                throw new ArgumentNullException(nameof(header), $"{nameof(header)} cannot be null.");
            }

            if (encoding is null)
            {
                throw new ArgumentNullException(nameof(encoding), $"{nameof(encoding)} cannot be null.");
            }

            var stringValue = encoding.GetString(header.GetValueBytes());

            return stringValue;
        }
    }
}
