using System;
using System.Collections.Generic;

namespace Confluent.Kafka.Core.Models.Internal
{
    using System.Text;

    internal static class HeadersExtensions
    {
        public static IDictionary<string, string> ToDictionary(this Headers headers, Encoding encoding = null)
        {
            if (headers is null)
            {
                throw new ArgumentNullException(nameof(headers), $"{nameof(headers)} cannot be null.");
            }

            var kafkaHeaders = new KafkaHeaders(headers, encoding);

            return kafkaHeaders;
        }
    }
}
