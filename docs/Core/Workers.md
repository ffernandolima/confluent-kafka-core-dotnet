| [Main](/README.md) > [Usage](/docs/Usage.md) > Workers |
|--------------------------------------------------------|

### Workers :cyclone:

Here's an example of how to use a Worker in your application:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaConsumerWorker<Null, string>((_, builder) =>
        builder.WithConsumer(builder =>
            builder.WithConsumerConfiguration(builder =>
                builder.WithBootstrapServers("localhost:9092")
                       .WithGroupId("test-worker-group")
                       .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                       .WithEnableAutoCommit(false)
                       .WithEnableAutoOffsetStore(false)
                       .WithTopicSubscriptions(["test-topic"])))
               .WithConsumeResultHandler(ConsumeResultHandler.Create())));

public sealed class ConsumeResultHandler : IConsumeResultHandler<Null, string>
{
    public static IConsumeResultHandler<Null, string> Create() => new ConsumeResultHandler();

    public Task HandleAsync(ConsumeResult<Null, string> consumeResult, CancellationToken cancellationToken)
    {
        // Add your processing logic here.
        return Task.CompletedTask;
    }
}
```

For enabling Dead Letter Topic (DLT) functionality, the following configuration should be added to the setup. This configuration routes faulted messages directly to the DLT for further inspection or handling:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaConsumerWorker<Null, string>((_, builder) =>
        // ... 
        builder.WithWorkerConfiguration(builder =>
            builder.WithEnableDeadLetterTopic(true)) // Enables DLT
                .WithDeadLetterProducer(builder =>
                    builder.WithProducerConfiguration(builder =>
                        builder.WithBootstrapServers("localhost:9092"))
                               .WithJsonCoreValueSerializer()))); // Set your desired serializer.
```
Note: DLT will only take effect if the retry topic is not enabled. If the retry topic is enabled, the DLT should be configured through the Retry Worker configuration instead.

By default, workers are going to be registered as **Singleton**.

In this configuration, a `BackgroundService` is automatically added and started when the host starts. This worker will run in the background and handle tasks like consuming Kafka messages, ensuring that your application processes data as soon as it's available, without needing manual intervention. For more information on hosted services and background workers, refer to the official [ASP.NET Core Hosted Services documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio).

### Worker Configurations :gear:

Some configurations should be pointed out as they enable custom behaviors:

| Configuration                           | Description                                                                                                                                                      |
|-----------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `MaxDegreeOfParallelism`                | The maximum degree of parallelism for message processing.                                                                                                        |
| `EnableLogging`                         | Enables logging for the worker.                                                                                                                                  |
| `EnableDiagnostics`                     | Enables diagnostics through `System.Diagnostics` for the worker.                                                                                                 |
| `CommitFaultedMessages`                 | Indicates whether to commit messages that have faulted during processing.                                                                                        |
| `EnableIdempotency`                     | Enables idempotency to ensure messages are processed exactly once (Idempotent Consumer). A `IIdempotencyHandler<TKey, TValue>` should be configured accordingly. |
| `EnableRetryOnFailure`                  | Enables retry logic when message processing fails. A `IRetryHandler<TKey, TValue>` should be configured accordingly.                                             |
| `EnableRetryTopic`                      | Enables retrying messages to a retry topic when message processing fails.                                                                                        |
| `RetryTopicExceptionTypeFilters`        | Filters exception types to decide which ones will be retried on a retry topic.                                                                                   |
| `RetryTopicExceptionFilter`             | A function to filter exceptions and decide which ones will be retried on a retry topic.                                                                          |
| `EnableDeadLetterTopic`                 | Enables sending failed messages to a dead-letter topic when processing fails.                                                                                    |
| `EnableMessageOrderGuarantee`           | Enables guaranteeing the order of message processing. A `MessageOrderGuaranteeKeyHandler` should be configured accordingly.                                      |
| `EmptyTopicDelay`                       | The delay when a topic has no messages to process.                                                                                                               |
| `NotEmptyTopicDelay`                    | The delay when a topic has messages to process.                                                                                                                  |
| `UnavailableProcessingSlotsDelay`       | The delay when processing slots are unavailable for message processing.                                                                                          |
| `ExceptionDelay`                        | The delay after an exception occurs during message processing.                                                                                                   |
| `PendingProcessingDelay`                | The delay when waiting for pending message processing.                                                                                                           |

### Retry Worker (Non-Blocking Retry) :repeat:

In cases where asynchronous retries through a retry topic are necessary, the following configuration enables a separate `BackgroundService` that manages the retry process in the background. This internal worker orchestrates the retry flow by re-sending faulted messages to the source topic for reprocessing. If Dead Letter Topic (DLT) functionality is enabled, messages that have exhausted all retry attempts are automatically routed to the DLT. If asynchronous retries are not needed, this configuration can be omitted. 

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaConsumerWorker<Null, string>((_, builder) =>
        // ... 
        builder.WithWorkerConfiguration(builder => 
            builder.WithEnableRetryTopic(true)) // Enables retry topic
                .WithRetryProducer(builder =>
                    builder.WithProducerConfiguration(builder =>
                        builder.WithBootstrapServers("localhost:9092"))
                               .WithJsonCoreValueSerializer())) // Set your desired serializer.
           .AddKafkaRetryConsumerWorker((_, builder) =>
               builder.WithConsumer(builder =>
                   builder.WithConsumerConfiguration(builder =>
                       builder.WithBootstrapServers("localhost:9092")
                              .WithGroupId("test-retry-worker-group")
                              .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                              .WithEnableAutoCommit(false)
                              .WithEnableAutoOffsetStore(false))
                          .WithJsonCoreValueDeserializer()) // Set your desired deserializer.
                      .WithSourceProducer(builder =>
                          builder.WithProducerConfiguration(builder =>
                              builder.WithBootstrapServers("localhost:9092")))));
```

By default, retry workers are going to be registered as **Singleton**.

For enabling Dead Letter Topic (DLT) functionality, the following configuration should be added to the setup. This ensures that messages which have exhausted all retry attempts are routed to the DLT for further inspection or handling:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaConsumerWorker<Null, string>((_, builder) => { /*...*/ })
           .AddKafkaRetryConsumerWorker((_, builder) =>
               // ...
               builder.WithWorkerConfiguration(builder =>
                   builder.WithEnableDeadLetterTopic(true)) // Enables DLT
                       .WithDeadLetterProducer(builder =>
                           builder.WithProducerConfiguration(builder =>
                               builder.WithBootstrapServers("localhost:9092"))
                                      .WithJsonCoreValueSerializer()))); // Set your desired deserializer.
```
Note: If DLT functionality is not required, this configuration can be omitted, similar to the retry topic configuration.

### Retry Worker Configurations :gear:

Some configurations should be pointed out as they enable custom behaviors:

| Configuration                          | Description                                                                                                                                                      |
|----------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `MaxDegreeOfParallelism`               | The maximum degree of parallelism for message processing.                                                                                                        |
| `EnableLogging`                        | Enables logging for the retry worker.                                                                                                                            |
| `EnableDiagnostics`                    | Enables diagnostics through `System.Diagnostics` for the retry worker.                                                                                           |
| `CommitFaultedMessages`                | Indicates whether to commit messages that have faulted during processing.                                                                                        |
| `EnableIdempotency`                    | Enables idempotency to ensure messages are processed exactly once (Idempotent Consumer). A `IIdempotencyHandler<TKey, TValue>` should be configured accordingly. |
| `EnableRetryOnFailure`                 | Enables retry logic when message processing fails. A `IRetryHandler<TKey, TValue>` should be configured accordingly.                                             |
| `EnableDeadLetterTopic`                | Enables sending failed messages to a dead-letter topic when processing has exhausted all retry attempts.                                                         |
| `RetryCount`                           | The number of times to retry message processing upon failure.                                                                                                    |
| `EmptyTopicDelay`                      | The delay when a topic has no messages to process.                                                                                                               |
| `NotEmptyTopicDelay`                   | The delay when a topic has messages to process.                                                                                                                  |
| `UnavailableProcessingSlotsDelay`      | The delay when processing slots are unavailable for message processing.                                                                                          |
| `ExceptionDelay`                       | The delay after an exception occurs during message processing.                                                                                                   |
| `PendingProcessingDelay`               | The delay when waiting for pending message processing.                                                                                                           |
| `RetryTopicDelay`                      | The delay before retrying message processing on a retry topic.                                                                                                   |

| [Go Back](/docs/Usage.md) |
|---------------------------| 