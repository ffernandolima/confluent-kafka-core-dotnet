| [Main](/README.md) > [Usage](/docs/Usage.md) > [Serialization](/docs/Serialization/Serialization.md) > protobuf-net |
|---------------------------------------------------------------------------------------------------------------------|

### protobuf-net

The protobuf-net integration allows you to leverage the protobuf-net library for serializing and deserializing messages in Kafka producers and consumers. This serializer offers flexibility and control over options for serializing data using protocol buffers to suit your application's needs.

### Installation

To install the package and start integrating with protobuf-net:
```bash
dotnet add package Confluent.Kafka.Core.Serialization.ProtobufNet
```

### Usage and Options Configuration

There are multiple ways to configure the protobuf-net serializer for your Kafka producer and consumer, allowing you to set the serializer for either the Key, the Value, or both, depending on your use case. The protobuf-net library offers some options for configuring how your messages are serialized using protocol buffers. These options can be passed through the protobuf-net serializer, providing fine-grained control over serialization and deserialization behavior. The options configuration is optional, and if not provided, default options will be assumed internally.

Here's an example for configuring a Kafka producer:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaProducer<Null, Message>((_, builder) =>
        // ...
            builder.WithProtobufNetValueSerializer(builder => 
                builder.WithAutomaticRuntimeMap(true) // Maps object fields and properties automatically
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
            builder.WithProtobufNetValueDeserializer(builder => 
                builder.WithAutomaticRuntimeMap(true) // Maps object fields and properties automatically
                     /*.With...*/))); // Additional options can be added here
```

### Configuration Methods

- `WithProtobufNetSerializer`: Sets the serializer for both the Key and Value.
- `WithProtobufNetKeySerializer`: Sets the serializer for the Key only.
- `WithProtobufNetValueSerializer`: Sets the serializer for the Value only.

- `WithProtobufNetDeserializer`: Sets the deserializer for both the Key and Value.
- `WithProtobufNetKeyDeserializer`: Sets the deserializer for the Key only.
- `WithProtobufNetValueDeserializer`: Sets the deserializer for the Value only.

| [Go Back](/docs/Serialization/Serialization.md) |
|-------------------------------------------------| 