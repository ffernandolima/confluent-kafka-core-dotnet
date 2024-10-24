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

By default, workers are going to be registered as ```Singleton```.

In this configuration, a BackgroundService is automatically added and started when the host starts. This worker will run in the background and handle tasks like consuming Kafka messages, ensuring that your application processes data as soon as it's available, without needing manual intervention. For more information on hosted services and background workers, refer to the official [ASP.NET Core Hosted Services documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio).

### Worker Configurations :gear:

Some configurations should be pointed out once they enable custom behaviors:

### Retry Worker :repeat:

For cases where asynchronous retries through a retry topic are desired, the following configuration should be applied. This configuration introduces an internal worker responsible for orchestrating the async retry process. It sends faulted messages to the source topic for reprocessing, or, if Dead Letter Topic (DLT) functionality is enabled, routes the failed messages directly to the DLT after exhausting all retry attempts. If asynchronous retries are not required, this configuration should be omitted.

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
                    .WithConsumeResultHandler(ConsumeResultHandler.Create())
                    .WithWorkerConfiguration(builder =>
                        builder.WithEnableRetryTopic(true)) // Enables retry topic
                    .WithRetryProducer(builder =>
                        builder.WithProducerConfiguration(builder =>
                            builder.WithBootstrapServers("localhost:9092"))
                                   .WithJsonCoreValueSerializer())) // Sets System.Text.Json as the serializer. Set your desired serializer.
                .AddKafkaRetryConsumerWorker((_, builder) =>
                    builder.WithConsumer(builder =>
                        builder.WithConsumerConfiguration(builder =>
                            builder.WithBootstrapServers("localhost:9092")
                                   .WithGroupId("test-retry-worker-group")
                                   .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                                   .WithEnableAutoCommit(false)
                                   .WithEnableAutoOffsetStore(false))
                               .WithJsonCoreValueDeserializer()) // Sets System.Text.Json as the deserializer. Set your desired deserializer.
                           .WithSourceProducer(builder =>
                               builder.WithProducerConfiguration(builder =>
                                   builder.WithBootstrapServers("localhost:9092")))));
```

### Retry Worker Configurations :gear:

Some configurations should be pointed out once they enable custom behaviors:

| [Go Back](/README.md) |
|-----------------------| 