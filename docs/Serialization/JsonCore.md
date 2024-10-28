| [Main](/README.md) > [Usage](/docs/Usage.md) > [Serialization](/docs/Serialization/Serialization.md) > JsonCore |
|-----------------------------------------------------------------------------------------------------------------|

### System.Text.Json (JsonCore) :outbox_tray:

The JsonCore integration allows you to leverage the System.Text.Json library for serializing and deserializing messages in Kafka producers and consumers. This serializer offers flexibility and control over JSON serialization options to suit your application's needs.

### Installation :hammer_and_wrench:

To install the package and start integrating with System.Text.Json:
```bash
dotnet add package Confluent.Kafka.Core.Serialization.JsonCore
```

### Usage and Options Configuration :jigsaw:

There are multiple ways to configure the JsonCore serializer for your Kafka producer and consumer, allowing you to set the serializer for either the Key, the Value, or both, depending on your use case. The System.Text.Json library offers many options for configuring how JSON is handled in your messages. These options can be passed through the JsonCore serializer, providing fine-grained control over serialization and deserialization behavior. The options configuration is optional, and if not provided, default options will be assumed internally.

Here's an example for configuring a Kafka producer:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaProducer<Null, Message>((_, builder) =>
        // ...
            builder.WithJsonCoreValueSerializer(builder => 
                builder.WithReferenceHandler(ReferenceHandler.IgnoreCycles) // Handles circular references
                       .WithPropertyNamingPolicy(JsonNamingPolicy.CamelCase) // Uses camelCase naming
                       .WithDefaultIgnoreCondition(JsonIgnoreCondition.WhenWritingNull) // Ignores null properties
                      /*.With...*/))); // Additional options can be added here
```

And here's an example for configuring a Kafka consumer:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaConsumer<Null, Message>((_, builder) =>
        // ...
            builder.WithJsonCoreValueDeserializer(builder => 
                builder.WithReferenceHandler(ReferenceHandler.IgnoreCycles) // Handles circular references
                       .WithPropertyNamingPolicy(JsonNamingPolicy.CamelCase) // Uses camelCase naming
                       .WithDefaultIgnoreCondition(JsonIgnoreCondition.WhenWritingNull) // Ignores null properties
                      /*.With...*/))); // Additional options can be added here
```

### Configuration Methods :nut_and_bolt:

- `WithJsonCoreSerializer`: Sets the serializer for both the Key and Value.
- `WithJsonCoreKeySerializer`: Sets the serializer for the Key only.
- `WithJsonCoreValueSerializer`: Sets the serializer for the Value only.

- `WithJsonCoreDeserializer`: Sets the deserializer for both the Key and Value.
- `WithJsonCoreKeyDeserializer`: Sets the deserializer for the Key only.
- `WithJsonCoreValueDeserializer`: Sets the deserializer for the Value only.

| [Go Back](/docs/Serialization/Serialization.md) |
|-------------------------------------------------| 