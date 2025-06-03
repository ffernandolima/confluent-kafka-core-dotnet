| [Main](/README.md) > [Usage](/docs/Usage.md) > Producers/Consumers |
|--------------------------------------------------------------------|

### Producers :factory:

Here's an example of how to use a Producer in your application:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaProducer<Null, string>((_, builder) =>
        builder.WithProducerConfiguration(builder =>
            builder.WithBootstrapServers("localhost:9092"))));

 using var serviceProvider = builder.Services.BuildServiceProvider();

 var producer = serviceProvider.GetRequiredService<IKafkaProducer<Null, string>>();

 // Synchronous production of a message
 producer.Produce("test-topic", new Message<Null, string> { Value = "test" });

 // Asynchronous production of a message
 await producer.ProduceAsync("test-topic", new Message<Null, string> { Value = "test" });
```

By default, producers are going to be registered as **Singleton**.

### Producer Configurations :gear:

Some configurations should be pointed out as they enable custom behaviors:

| Configuration                           | Description                                                                                                                                             |
|-----------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------|
| `DefaultTopic`                          | The default topic where messages will be produced.                                                                                                      |
| `DefaultPartition`                      | The default partition where messages will be produced.                                                                                                  |
| `DefaultTimeout`                        | The default timeout for producing messages.                                                                                                             |
| `PollAfterProducing`                    | Indicates whether to poll the broker after producing a message.                                                                                         |
| `EnableLogging`                         | Enables logging for the producer.                                                                                                                       |
| `EnableDiagnostics`                     | Enables diagnostics through `System.Diagnostics` for the producer.                                                                                      |
| `EnableRetryOnFailure`                  | Enables retry logic when message production fails. A `IRetryHandler<TKey, TValue>` should be configured accordingly.                                    |
| `EnableInterceptorExceptionPropagation` | Enables the propagation of exceptions thrown by interceptors.  A `IKafkaProducerInterceptor<TKey, TValue>` collection should be configured accordingly. |

### Consumers :zap:

Here's an example of how to use a Consumer in your application:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaConsumer<Null, string>((_, builder) =>
       builder.WithConsumerConfiguration(builder =>
           builder.WithBootstrapServers("localhost:9092")
                  .WithGroupId("test-consumer-group")
                  .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                  .WithTopicSubscriptions(["test-topic"])));

 using var serviceProvider = builder.Services.BuildServiceProvider();

 var consumer = serviceProvider.GetRequiredService<IKafkaConsumer<Null, string>>();

 // Single consumption
 var consumeResult = consumer.Consume();
 
 // Batch consumption
 var consumeResults = consumer.ConsumeBatch();
```

By default, consumers are going to be registered as **Singleton**.

### Consumer Configurations :gear:

Some configurations should be pointed out as they enable custom behaviors:

| Configuration                           | Description                                                                                                                                            |
|-----------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------|
| `TopicSubscriptions`                    | The default topics where messages will be consumed.                                                                                                    |
| `PartitionAssignments`                  | The default partitions where messages will be consumed.                                                                                                |
| `CommitAfterConsuming`                  | Indicates whether to commit offsets after consuming a message.                                                                                         |
| `DefaultTimeout`                        | The default timeout for consuming messages.                                                                                                            |
| `DefaultBatchSize`                      | The default batch size for consuming messages.                                                                                                         |
| `EnableLogging`                         | Enables logging for the consumer.                                                                                                                      |
| `EnableDiagnostics`                     | Enables diagnostics through `System.Diagnostics` for the consumer.                                                                                     |
| `EnableDeadLetterTopic`                 | Enables sending failed messages to a dead-letter topic when consumption fails.                                                                         |
| `EnableRetryOnFailure`                  | Enables retry logic when message consumption fails. A `IRetryHandler<TKey, TValue>` should be configured accordingly.                                  |
| `EnableInterceptorExceptionPropagation` | Enables the propagation of exceptions thrown by interceptors. A `IKafkaConsumerInterceptor<TKey, TValue>` collection should be configured accordingly. |

| [Go Back](/docs/Usage.md) |
|---------------------------| 