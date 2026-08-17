| [Main](/README.md) > [Usage](/docs/Usage.md) > Distributed Tracing and OpenTelemetry |
|--------------------------------------------------------------------------------------|

### Distributed Tracing and OpenTelemetry :globe_with_meridians:

### Features :bulb:

- **Distributed Tracing**: Utilizes the `System.Diagnostics` implementation for tracing, which is part of the Kafka Core.
- **OpenTelemetry Integration**: Registers the `Confluent.Kafka.Core` source with the `TracerProviderBuilder` from the OpenTelemetry API.
- **Automatic Semantic Conventions**: Spans are tagged automatically, following the OpenTelemetry semantic conventions for messaging. The attributes emitted are listed below.
- **Metrics**: The four messaging metrics are emitted from the `Confluent.Kafka.Core` meter.

### Installation :hammer_and_wrench:

The Distributed Tracing implementation is part of the Kafka Core.

To install the package and start integrating with OpenTelemetry:
```bash
dotnet add package Confluent.Kafka.Core.OpenTelemetry
```

### Usage and Custom Enrichment :jigsaw:

To enable distributed tracing, call the `AddKafkaDiagnostics` method while registering Kafka Core services into the Microsoft built-in container. Below are some examples:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder => builder.AddKafkaDiagnostics());
```

The tracing can be customized by using options to add custom tags. Here's an example:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder => 
    builder.AddKafkaDiagnostics(builder =>
        builder.WithConsumptionEnrichment((activity, context) => 
            activity.SetTag("custom-consumption-tag", "consumption-value"))
             /*.With...*/)); // Additional options can be added here
```

While it's not required to add custom tags, the options provided can be used to enhance tracings with additional information.

To integrate with OpenTelemetry:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithTracing(builder => builder.AddKafkaCoreInstrumentation()); // Adds Confluent.Kafka.Core source 
```

### Semantic Conventions :open_book:

The library follows the OpenTelemetry semantic conventions for messaging, **v1.44.0**. For more information, refer to the following links:

- [OpenTelemetry Messaging Spans](https://github.com/open-telemetry/semantic-conventions/blob/v1.44.0/docs/messaging/messaging-spans.md)
- [OpenTelemetry Kafka Span Attributes](https://github.com/open-telemetry/semantic-conventions/blob/v1.44.0/docs/messaging/kafka.md#span-attributes)

> [!NOTE]
> The messaging conventions are still marked *Development* upstream, so these attribute names may
> change again in a future release.

Spans are named `{operation} {topic}` — for example `publish orders`, `receive orders`,
`process orders`. Producing and processing use the `Producer` and `Consumer` activity kinds; a
pull-based receive uses `Client`, as the conventions require.

Attributes emitted:

| Attribute | Notes |
|---|---|
| `messaging.system` | always `kafka` |
| `messaging.operation.name` | `publish`, `receive` or `process` |
| `messaging.operation.type` | `send`, `receive` or `process` — note publishing maps to `send` |
| `messaging.client.id` | your configured `ClientId`, or `rdkafka` when unset |
| `messaging.destination.name` | topic |
| `messaging.destination.partition.id` | partition, as a string |
| `messaging.consumer.group.name` | consumers only |
| `messaging.message.id` | when a message id handler is configured |
| `messaging.message.body.size` | |
| `messaging.kafka.offset` | |
| `messaging.kafka.message.key` | when the message has a key |
| `messaging.kafka.message.tombstone` | only when `true` |
| `server.address`, `server.port` | first bootstrap server |
| `error.type` | on failure only |

Exceptions are recorded as span events rather than attributes.

The following are specific to this library and are not part of the OpenTelemetry conventions. They
carry Kafka error detail that `error.type` cannot express:
`messaging.kafka.result.is_error`, `messaging.kafka.result.error_code`,
`messaging.kafka.result.error_reason` and `messaging.kafka.processing.is_error`.

### Metrics :bar_chart:

Metrics are emitted from the `Confluent.Kafka.Core` meter. Register it alongside the tracing source:

```C#
builder.Services.AddOpenTelemetry()
    .WithMetrics(builder => builder.AddKafkaCoreInstrumentation()); // Adds Confluent.Kafka.Core meter
```

There is no configuration to turn metrics on. Recording is nearly free while no `MeterProvider`
listens, so registering the meter is the opt-in. Setting `EnableDiagnostics` to `false` disables
tracing and metrics together.

The library emits all four metrics defined by the messaging conventions, following
[messaging-metrics.md](https://github.com/open-telemetry/semantic-conventions/blob/v1.44.0/docs/messaging/messaging-metrics.md):

| Metric | Instrument | Unit | Recorded on |
|---|---|---|---|
| `messaging.client.operation.duration` | Histogram | `s` | producing and consuming |
| `messaging.process.duration` | Histogram | `s` | worker processing |
| `messaging.client.sent.messages` | Counter | `{message}` | producing |
| `messaging.client.consumed.messages` | Counter | `{message}` | consuming |

Attributes are a subset of the span attributes: `messaging.system`, `messaging.operation.name`,
`messaging.operation.type`, `messaging.consumer.group.name` (consumers), `messaging.destination.name`,
`messaging.destination.partition.id`, `server.address`, `server.port`, and `error.type` on failure.

The counters measure attempts, so a failed operation is counted and told apart by `error.type`.
An empty poll and a partition EOF are not message deliveries, so neither is recorded.

> [!NOTE]
> Metrics are recorded independently of the spans. A span is absent when nothing is listening and
> when sampling drops it, so metrics derived from one would undercount in proportion to the sampling
> rate.

Consumer lag is deliberately absent: it is not part of the OpenTelemetry messaging conventions, and
it cannot be derived without the broker high-watermark. Use `StatisticsIntervalMs` with a statistics
handler if you need it today.

### Additional Resources :spiral_notepad:

- [Microsoft Distributed Tracing Documentation](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing)

| [Go Back](/docs/Usage.md) |
|---------------------------|  