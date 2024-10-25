| [Main](/README.md) > [Usage](/docs/Usage.md) > [Serialization](/docs/Serialization/Serialization.md) > JsonCore |
|-----------------------------------------------------------------------------------------------------------------|

### System.Text.Json (JsonCore)

The JsonCore integration allows you to leverage the System.Text.Json library for serializing and deserializing messages in Kafka producers and consumers. This serializer offers flexibility and control over JSON serialization options to suit your application's needs.

### Installation

To install the package and start integrating with System.Text.Json:
```bash
dotnet add package Confluent.Kafka.Core.Serialization.JsonCore
```

### Usage

There are multiple ways to configure the JsonCore serializer for your Kafka producer. You can set the serializer for either the Key, the Value, or both, depending on your use case.

```C#
 IServiceCollection services = new ServiceCollection()
     .AddKafka(builder =>
         builder.AddKafkaProducer<Null, Message>((_, builder) =>
             builder.WithProducerConfiguration(builder =>
                 builder.WithBootstrapServers("localhost:9092"))
                     .WithJsonCoreValueSerializer())); // Sets only the value serializer since the Key is Null.

```

### Options Configuration 

The System.Text.Json library provides many options for configuring how JSON is handled in your messages. These options can be passed through the JsonCore serializer, giving you fine-grained control over serialization behavior.

Here's an example of configuring the JsonSerializerOptions:

```C#
 IServiceCollection services = new ServiceCollection()
     .AddKafka(builder =>
         builder.AddKafkaProducer<Null, Message>((_, builder) =>
             builder.WithProducerConfiguration(builder =>
                 builder.WithBootstrapServers("localhost:9092"))
                     .WithJsonCoreValueSerializer(builder => 
                         builder.WithReferenceHandler(ReferenceHandler.IgnoreCycles) // Handles circular references
                                .WithPropertyNamingPolicy(JsonNamingPolicy.CamelCase) // Uses camelCase naming
                                .WithDefaultIgnoreCondition(JsonIgnoreCondition.WhenWritingNull) // Ignores null properties
                              /*.With...*/));
                               
```

### Configuration Methods

- WithJsonCoreSerializer: Sets the serializer for both the Key and Value.
- WithJsonCoreKeySerializer: Sets the serializer for the Key only.
- WithJsonCoreValueSerializer: Sets the serializer for the Value only.

| [Go Back](/docs/Serialization/Serialization.md) |
|-------------------------------------------------| 