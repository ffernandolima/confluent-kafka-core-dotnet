using System;

namespace Confluent.Kafka.Core.Producer
{
    public interface IKafkaProducerHandlerFactory<TKey, TValue>
    {
        Action<IProducer<TKey, TValue>, string> CreateStatisticsHandler();

        Action<IProducer<TKey, TValue>, Error> CreateErrorHandler();

        Action<IProducer<TKey, TValue>, LogMessage> CreateLogHandler();

        Func<IKafkaProducer<TKey, TValue>, object> CreateProducerIdHandler();

        Func<TValue, object> CreateMessageIdHandler();
    }
}
