| [Main](/README.md) > [Usage](/docs/Usage.md) > Workers |
|--------------------------------------------------------|

### Workers :cyclone:

Here's an example of how to use a Worker in your .NET application:

```C#
public sealed class ConsumeResultHandler : IConsumeResultHandler<Null, string>
{
    public static IConsumeResultHandler<Null, string> Create() => new ConsumeResultHandler();

    public Task HandleAsync(ConsumeResult<Null, string> consumeResult, CancellationToken cancellationToken)
    {
        // Add your processing logic here.
        return Task.CompletedTask;
    }
}

 IServiceCollection services = new ServiceCollection()
     .AddKafka(builder =>
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
```

For enabling Dead Letter Topic (DLT) functionality, the following configuration should be added to the setup. This configuration routes faulted messages directly to the DLT for further inspection or handling:

```C#
IServiceCollection services = new ServiceCollection()
     .AddKafka(builder =>
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

In this configuration, a BackgroundService is automatically added and started when the host starts. This worker will run in the background and handle tasks like consuming Kafka messages, ensuring that your application processes data as soon as it's available, without needing manual intervention. For more information on hosted services and background workers, refer to the official [ASP.NET Core Hosted Services documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio).

### Worker Configurations :gear:

Some configurations should be pointed out once they enable custom behaviors:

### Retry Worker :repeat:

In cases where asynchronous retries through a retry topic are necessary, the following configuration enables a separate BackgroundService that manages the retry process in the background. This internal worker orchestrates the retry flow by re-sending faulted messages to the source topic for reprocessing. If Dead Letter Topic (DLT) functionality is enabled, messages that have exhausted all retry attempts are automatically routed to the DLT. If asynchronous retries are not needed, this configuration can be omitted. 

```C#
 IServiceCollection services = new ServiceCollection()
     .AddKafka(builder =>
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
 IServiceCollection services = new ServiceCollection()
     .AddKafka(builder =>
         builder.AddKafkaConsumerWorker<Null, string>((_, builder) => 
             {
                 // ...
             }).AddKafkaRetryConsumerWorker((_, builder) =>
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

Some configurations should be pointed out once they enable custom behaviors:

| [Go Back](/README.md) |
|-----------------------| 