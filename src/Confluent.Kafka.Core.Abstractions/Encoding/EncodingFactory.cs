using System;

namespace Confluent.Kafka.Core.Encoding
{
    using System.Text;

    public sealed class EncodingFactory : IEncodingFactory
    {
        private static readonly Lazy<EncodingFactory> Factory = new(
          () => new EncodingFactory(), isThreadSafe: true);

        public static EncodingFactory Instance => Factory.Value;

        private EncodingFactory()
        { }

        public Encoding CreateDefault() => new UTF8Encoding(
            encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes: false);
    }
}
