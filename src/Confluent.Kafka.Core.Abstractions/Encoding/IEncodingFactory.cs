namespace Confluent.Kafka.Core.Encoding
{
    using System.Text;

    public interface IEncodingFactory
    {
        Encoding CreateDefault();
    }
}
