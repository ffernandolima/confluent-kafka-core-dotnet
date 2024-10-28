| [Main](/README.md) > [Usage](/docs/Usage.md) > [Serialization](/docs/Serialization/Serialization.md) > Json.NET |
|-----------------------------------------------------------------------------------------------------------------|

### Newtonsoft.Json (Json.NET) :outbox_tray:

The Json.NET integration allows you to leverage the Newtonsoft.Json library for serializing and deserializing messages in Kafka producers and consumers. This serializer offers flexibility and control over JSON serialization settings to suit your application's needs.

### Installation :hammer_and_wrench:

To install the package and start integrating with Newtonsoft.Json:
```bash
dotnet add package Confluent.Kafka.Core.Serialization.NewtonsoftJson
```

### Usage and Settings Configuration :jigsaw:

There are multiple ways to configure the Json.NET serializer for your Kafka producer and consumer, allowing you to set the serializer for either the Key, the Value, or both, depending on your use case. The Newtonsoft.Json library offers many settings for configuring how JSON is handled in your messages. These settings can be passed through the Json.NET serializer, providing fine-grained control over serialization and deserialization behavior. The settings configuration is optional, and if not provided, default settings will be assumed internally.

Here's an example for configuring a Kafka producer:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaProducer<Null, Message>((_, builder) =>
        // ...
            builder.WithNewtonsoftJsonValueSerializer(builder => 
                builder.WithNullValueHandling(NullValueHandling.Ignore) // Ignores null values
                       .WithReferenceLoopHandling(ReferenceLoopHandling.Ignore) // Ignores reference loops
                       .WithMetadataPropertyHandling(MetadataPropertyHandling.Ignore) // Ignores metadata properties
                       .WithDateFormatHandling(DateFormatHandling.IsoDateFormat) // Uses ISO date format
                       .WithContractResolver(new CamelCasePropertyNamesContractResolver()) // Camel case property names
                       .WithConverters([new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }]) // Custom date handling
                     /*.With...*/)); // Additional settings can be added here
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
            builder.WithNewtonsoftJsonValueDeserializer(builder => 
                builder.WithNullValueHandling(NullValueHandling.Ignore) // Ignores null values
                       .WithReferenceLoopHandling(ReferenceLoopHandling.Ignore) // Ignores reference loops
                       .WithMetadataPropertyHandling(MetadataPropertyHandling.Ignore) // Ignores metadata properties
                       .WithDateFormatHandling(DateFormatHandling.IsoDateFormat) // Uses ISO date format
                       .WithContractResolver(new CamelCasePropertyNamesContractResolver()) // Camel case property names
                       .WithConverters([new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }]) // Custom date handling
                     /*.With...*/)); // Additional settings can be added here
```

### Configuration Methods :nut_and_bolt:

- `WithNewtonsoftJsonSerializer`: Sets the serializer for both the Key and Value.
- `WithNewtonsoftJsonKeySerializer`: Sets the serializer for the Key only.
- `WithNewtonsoftJsonValueSerializer`: Sets the serializer for the Value only.

- `WithNewtonsoftJsonDeserializer`: Sets the deserializer for both the Key and Value.
- `WithNewtonsoftJsonKeyDeserializer`: Sets the deserializer for the Key only.
- `WithNewtonsoftJsonValueDeserializer`: Sets the deserializer for the Value only.

| [Go Back](/docs/Serialization/Serialization.md) |
|-------------------------------------------------| 