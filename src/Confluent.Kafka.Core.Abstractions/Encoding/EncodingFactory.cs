namespace Confluent.Kafka.Core.Encoding
{
    using System.Text;

    public static class EncodingFactory
    {
        public static Encoding CreateDefault() => new UTF8Encoding(
            encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes: false);
    }
}
