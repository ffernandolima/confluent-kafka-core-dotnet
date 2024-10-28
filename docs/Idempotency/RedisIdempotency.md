| [Main](/README.md) > [Usage](/docs/Usage.md) > Idempotency |
|------------------------------------------------------------|

### Idempotency (Redis Provider) :recycle:

The Idempotent Consumer leverages Redis to ensure message processing idempotency. This functionality is especially beneficial in scenarios where avoiding duplicate message processing in Kafka is essential.

By implementing Redis-based idempotency, the Idempotent Consumer can track which messages have been processed using unique identifiers. When a message is received, the consumer checks if it has already been processed based on its identifier. If it has, the consumer can skip processing, ensuring that the system remains consistent and reliable.

### Installation :hammer_and_wrench:

To install the package and start integrating with Redis:
```bash
dotnet add package Confluent.Kafka.Core.Idempotency.Redis
```

### Usage and Configuration :bar_chart:
To configure the idempotency handler, use the `WithRedisIdempotencyHandler` method. This handler can be added to both worker and retry worker for idempotent processing.

To add the Redis idempotency handler to a Kafka worker:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaConsumerWorker<Null, Message>((_, builder) =>
        // ...
        builder.WithRedisIdempotencyHandler(builder =>
            builder.WithRedisClient(builder =>
                builder.WithEndPoints([EndPointCollection.TryParse("localhost:6379")])
                       .WithAbortOnConnectFail(false)
                       /*.With...*/) // Additional options can be added here
            .WithHandlerOptions(builder =>
                builder.WithGroupId("test-group")
                       .WithConsumerName("test-consumer")
                       .WithMessageIdHandler(message => message.Id)
                       /*.With...*/)))); // Additional options can be added here
```

To add the Redis idempotency handler to a Kafka retry worker:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaRetryConsumerWorker((_, builder) =>
        // ...
        builder.WithRedisIdempotencyHandler(builder =>
            builder.WithRedisClient(builder =>
                builder.WithEndPoints([EndPointCollection.TryParse("localhost:6379")])
                       .WithAbortOnConnectFail(false)
                       /*.With...*/) // Additional options can be added here
            .WithHandlerOptions(builder =>
                builder.WithGroupId("test-group")
                       .WithConsumerName("test-consumer")
                       .WithMessageIdHandler(message => message.Id.ToString())
                       /*.With...*/)))); // Additional options can be added here
```

### Recommended Interface for Message Value :envelope_with_arrow:

It is strongly recommended to implement the `IMessageValue` interface from the namespace `Confluent.Kafka.Core.Models` within the message value to create a standard and simplify message Id discovery. This interface includes the `Id` property as a `Guid` and can be used with the `MessageIdHandler`.

### Idempotency Handler Configurations :gear:

Some configurations should be pointed out as they enable custom behaviors:

| Property             | Description                                                                                                                                                                                   |
|----------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `GroupId`            | The identifier for the consumer group to which this handler belongs.                                                                                                                          |
| `ConsumerName`       | The name of the consumer to which this handler belongs.                                                                                                                                       |
| `ExpirationInterval` | The duration after which processed messages are considered expired and eligible for removal. Default is `7 days`.                                                                             |
| `ExpirationDelay`    | The delay used by the background task that processes eligible messages for removal. This delay is the period the task waits before checking for new eligible messages. Default is `1 minute`. |
| `MessageIdHandler`   | A function that extracts the unique message Id from the message value.                                                                                                                        |
| `EnableLogging`      | Indicates whether logging is enabled for this handler. Default is `true`.                                                                                                                     |

Note: `GroupId` and `ConsumerName` will be used to create a unique key for storing processed messages in Redis, formatted as:
`Idempotency:GroupIds:{GroupId}:Consumers:{ConsumerName}.`

| [Go Back](/docs/Usage.md) |
|---------------------------| 