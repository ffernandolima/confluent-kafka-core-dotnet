| [Main](/README.md) > [Usage](/docs/Usage.md) > Distributed Tracing and OpenTelemetry |
|--------------------------------------------------------------------------------------|

### Distributed Tracing and OpenTelemetry :globe_with_meridians:

### Features :bulb:

- **Distributed Tracing**: Utilizes the `System.Diagnostics` implementation for tracing, which is part of the Kafka Core.
- **OpenTelemetry Integration**: Registers the `Confluent.Kafka.Core` source with the `TracerProviderBuilder` from the OpenTelemetry API.
- **Automatic Semantic Conventions**: By default, the library adds all tags from the OpenTelemetry semantic conventions for messaging.

### Installation :hammer_and_wrench:

The Distributed Tracing implementation is part of the Kafka Core.

To install the package and start integrating with OpenTelemetry:
```bash
dotnet add package Confluent.Kafka.Core.OpenTelemetry
```

### Usage and Custom Enrichment :bar_chart:

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

builder.Services.AddKafka(builder => builder.AddKafkaDiagnostics(builder =>
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

The library automatically includes all the tags defined by the OpenTelemetry semantic conventions for messaging. For more information, refer to the following links:

- [OpenTelemetry Messaging Spans](https://github.com/open-telemetry/semantic-conventions/blob/v1.23.1/docs/messaging/messaging-spans.md)
- [OpenTelemetry Kafka Span Attributes](https://github.com/open-telemetry/semantic-conventions/blob/v1.23.1/docs/messaging/kafka.md#span-attributes)

### Additional Resources :spiral_notepad:

- [Microsoft Distributed Tracing Documentation](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing)

| [Go Back](/docs/Usage.md) |
|---------------------------|  