| [Main](/README.md) > [Usage](/docs/Usage.md) > [Serialization](/docs/Serialization/Serialization.md) > SchemaRegistry.Avro |
|----------------------------------------------------------------------------------------------------------------------------|

### SchemaRegistry.Avro :outbox_tray:

The SchemaRegistry.Avro integration allows you to leverage the Confluent.SchemaRegistry.Serdes.Avro library for serializing and deserializing messages in Kafka producers and consumers. This serializer ensures schema compatibility and version management, providing a flexible and efficient way to handle Avro-encoded messages.

### Installation :hammer_and_wrench:

To install the package and start integrating with Confluent.SchemaRegistry.Serdes.Avro:
```bash
dotnet add package Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro
```

### Usage and Configuration :jigsaw:

There are multiple ways to configure the SchemaRegistry.Avro serializer for your Kafka producer and consumer, allowing you to set the serializer for either the Key, the Value, or both, depending on your use case. It provides several configurations for controlling how your messages interact with the Confluent Schema Registry. These configurations can be passed through the SchemaRegistry.Avro serializer, allowing for fine-grained control over schema registration, compatibility checks, and both serialization and deserialization behaviors. The Schema Registry client configuration is required, but the other configurations are optional, and if not provided, default configurations will be assumed internally.

Here's an example for configuring a Kafka producer:

```C#
// Web
var builder = WebApplication.CreateBuilder(args);

// Non-Web
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddKafka(builder =>
    builder.AddKafkaProducer<Null, Message>((_, builder) =>
        // ...
            builder.WithSchemaRegistryAvroValueSerializer(builder =>
                builder.WithSchemaRegistryClient(builder =>
                    builder.WithSchemaRegistryConfiguration(builder =>
                        builder.WithUrl("http://localhost:8081"))) // Configures Schema Registry client
                       .WithSerializerConfiguration(builder =>
                           builder.WithAutoRegisterSchemas(true)) // Automatically registers new schemas
                     /*.With...*/))); // Additional configs can be added here
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
            builder.WithSchemaRegistryAvroValueDeserializer(builder =>
                builder.WithSchemaRegistryClient(builder =>
                    builder.WithSchemaRegistryConfiguration(builder =>
                        builder.WithUrl("http://localhost:8081"))) // Configures Schema Registry client
                       .WithDeserializerConfiguration(builder =>
                           { /*...*/ }) // Additional deserializer configs can be added here                               
                      /*.With...*/))); // Additional configs can be added here
```

### Configuration Methods :nut_and_bolt:

- `WithSchemaRegistryAvroSerializer`: Sets the serializer for both the Key and Value.
- `WithSchemaRegistryAvroKeySerializer`: Sets the serializer for the Key only.
- `WithSchemaRegistryAvroValueSerializer`: Sets the serializer for the Value only.

- `WithSchemaRegistryAvroDeserializer`: Sets the deserializer for both the Key and Value.
- `WithSchemaRegistryAvroKeyDeserializer`: Sets the deserializer for the Key only.
- `WithSchemaRegistryAvroValueDeserializer`: Sets the deserializer for the Value only.

| [Go Back](/docs/Serialization/Serialization.md) |
|-------------------------------------------------| 