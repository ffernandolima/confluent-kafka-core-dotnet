| [Main](/README.md) > [Usage](/docs/Usage.md) > [Serialization](/docs/Serialization/Serialization.md) > Json.NET |
|-----------------------------------------------------------------------------------------------------------------|

### Newtonsoft.Json (Json.NET)

The Json.NET integration allows you to leverage the Newtonsoft.Json library for serializing and deserializing messages in Kafka producers and consumers. This serializer offers flexibility and control over JSON serialization settings to suit your application's needs.

### Installation

To install the package and start integrating with Newtonsoft.Json:
```bash
dotnet add package Confluent.Kafka.Core.Serialization.NewtonsoftJson
```

### Usage

There are multiple ways to configure the Json.NET serializer for your Kafka producer. You can set the serializer for either the Key, the Value, or both, depending on your use case.

```C#
 IServiceCollection services = new ServiceCollection()
     .AddKafka(builder =>
         builder.AddKafkaProducer<Null, Message>((_, builder) =>
             builder.WithProducerConfiguration(builder =>
                 builder.WithBootstrapServers("localhost:9092"))
                     .WithNewtonsoftJsonValueSerializer())); // Sets only the value serializer since the Key is Null.

```

### Settings Configuration

The Newtonsoft.Json library provides many settings for configuring how JSON is handled in your messages. These settings can be passed through the Json.NET serializer, giving you fine-grained control over serialization behavior.

Here's an example of configuring the JsonSerializerSettings:

```C#
 IServiceCollection services = new ServiceCollection()
     .AddKafka(builder =>
         builder.AddKafkaProducer<Null, Message>((_, builder) =>
             builder.WithProducerConfiguration(builder =>
                 builder.WithBootstrapServers("localhost:9092"))
                     .WithNewtonsoftJsonValueSerializer(builder => 
                         builder.WithNullValueHandling(NullValueHandling.Ignore) // Ignores null values
                                .WithReferenceLoopHandling(ReferenceLoopHandling.Ignore) // Ignores reference loops
                                .WithMetadataPropertyHandling(MetadataPropertyHandling.Ignore) // Ignores metadata properties
                                .WithDateFormatHandling(DateFormatHandling.IsoDateFormat) // Uses ISO date format
                                .WithContractResolver(new CamelCasePropertyNamesContractResolver()) // Camel case property names
                                .WithConverters([new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }]) // Custom date handling
                              /*.With...*/));
                               
```

### Configuration Methods

- WithNewtonsoftJsonSerializer: Sets the serializer for both the Key and Value.
- WithNewtonsoftJsonKeySerializer: Sets the serializer for the Key only.
- WithNewtonsoftJsonValueSerializer: Sets the serializer for the Value only.

| [Go Back](/docs/Serialization/Serialization.md) |
|-------------------------------------------------| 