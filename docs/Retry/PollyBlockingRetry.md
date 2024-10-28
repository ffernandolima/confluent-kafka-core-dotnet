| [Main](/README.md) > [Usage](/docs/Usage.md) > Blocking Retry |
|---------------------------------------------------------------|

### Blocking Retry (Polly Provider) :repeat:

A flexible retry mechanism is available through Polly, allowing configuration of strategies such as immediate retries, fixed delays, and exponential backoff. This retry handler can be seamlessly integrated into Producers, Consumers, Workers, and Retry Workers through builders, ensuring smooth handling of transient issues.

### Installation :hammer_and_wrench:

To install the package and start integrating with Polly:
```bash
dotnet add package Confluent.Kafka.Core.Retry.Polly
```

### Usage and Configuration :jigsaw:

To configure the retry handler, use the `WithPollyRetryHandler` method, which is applied to Producers, Consumers, Workers, and Retry Workers via the builder pattern.

In the examples below, the Add... placeholder can be replaced with:

- `AddKafkaProducer` for Producers.
- `AddKafkaConsumer` for Consumers.
- `AddKafkaConsumerWorker` for Consumer Workers.
- `AddKafkaRetryConsumerWorker` for Retry Workers.

These examples illustrate how to configure immediate retries, fixed delays, exponential backoff, and a few other variations that might be useful.

#### Immediate Retry

This example demonstrates an immediate retry strategy, where the operation is retried instantly without any delay between attempts.

```C#
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.Add...((_, builder) =>
        // ... 
        builder.WithPollyRetryHandler(builder => 
            builder.WithDelayProvider(_ => TimeSpan.Zero) // No delay
                   .WithRetryCount(3) // Retry up to 3 times
                 /*.With...*/));
```

#### Fixed Delay

This example demonstrates a fixed delay strategy, where retries occur with a consistent 5-second delay between attempts.

```C#
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.Add...((_, builder) =>
        // ... 
        builder.WithPollyRetryHandler(builder => 
            builder.WithDelayProvider(_ => TimeSpan.FromSeconds(5)) // Fixed 5 seconds delay
                   .WithRetryCount(3) // Retry up to 3 times
                 /*.With...*/));
```

#### Exponential Backoff

This example demonstrates an exponential backoff strategy, where the delay increases exponentially with each retry attempt, starting with a 100-millisecond delay.

```C#
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.Add...((_, builder) =>
        // ... 
        builder.WithPollyRetryHandler(builder => 
            builder.WithDelayProvider(retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100)) // Exponential backoff
                   .WithRetryCount(5) // Retry up to 5 times
                 /*.With...*/));
```

#### Linear Backoff

This example demonstrates a linear backoff strategy, where the delay increases by a constant amount (1 second) after each retry attempt.

```C#
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.Add...((_, builder) =>
        // ... 
        builder.WithPollyRetryHandler(builder => 
            builder.WithDelayProvider(retryAttempt => TimeSpan.FromSeconds(retryAttempt)) // Linear backoff (1s, 2s, 3s, ...)
                   .WithRetryCount(3) // Retry up to 3 times
                 /*.With...*/));
```

#### Retry Forever

This example demonstrates a retry forever strategy, where the operation is retried indefinitely with a fixed delay of 1 second between each attempt.

```C#
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.Add...((_, builder) =>
        // ... 
        builder.WithPollyRetryHandler(builder => 
            builder.WithDelayProvider(_ => TimeSpan.FromSeconds(1)) // 1 second delay between retries
                   .WithRetryCount(int.MaxValue) // Retry indefinitely
                 /*.With...*/));
```

#### Conditional Retry

This example demonstrates a conditional retry strategy, where retries are only attempted if specific exceptions are thrown, such as `TimeoutException`.

```C#
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.Add...((_, builder) =>
        // ... 
        builder.WithPollyRetryHandler(builder => 
            builder.WithExceptionFilter(ex => ex is TimeoutException) // Retry only on TimeoutException
                   .WithDelayProvider(_ => TimeSpan.FromSeconds(2)) // 2 seconds delay
                   .WithRetryCount(3) // Retry up to 3 times
                 /*.With...*/));
```

### Retry Handler Configurations :gear:

| **Configuration**      | **Description**                                                                                 |
|------------------------|-------------------------------------------------------------------------------------------------|
| `RetryCount`           | The number of retry attempts. Default is `1`.                                                   |
| `RetryDelay`           | The fixed delay between retries. Default is no delay (`TimeSpan.Zero`).                         |
| `Delays`               | A sequence of custom delays for retries.                                                        |
| `DelayProvider`        | A function to provide dynamic delays based on the retry attempt number.                         |
| `ExceptionTypeFilters` | Indicates which exception types should trigger a retry.                                         |
| `ExceptionFilter`      | A function to filter which exceptions should trigger a retry.                                   |
| `EnableLogging`        | Indicates whether logging is enabled for this handler. Default is `true`.                       |


| [Go Back](/docs/Usage.md) |
|---------------------------| 