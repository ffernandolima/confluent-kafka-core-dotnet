namespace Confluent.Kafka.Core.Encoding
{
    using System.Text;

    public interface IEncodingFactory
    {
        static Encoding Create() => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: false);
    }
}
