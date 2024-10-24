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
                         .WithGroupId("test-consumer-group")
                         .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                         .WithTopicSubscriptions(["test-topic"])))
                    .WithConsumeResultHandler(ConsumeResultHandler.Create()));
```

By default, workers are going to be registered as ```Singleton```.

In this configuration, a BackgroundService is automatically added and started when the host starts. This worker will run in the background and handle tasks like consuming Kafka messages, ensuring that your application processes data as soon as it's available, without needing manual intervention. For more information on hosted services and background workers, refer to the official [ASP.NET Core Hosted Services documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-8.0&tabs=visual-studio).

| [Go Back](/README.md) |
|-----------------------| 