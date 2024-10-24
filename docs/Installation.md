| [Main](/README.md) > Installation |
|-----------------------------------|

### Installation :hammer_and_wrench:

1. **Confluent.Kafka.Core**  
   For building Kafka producers and consumers. This package includes the core functionalities necessary to interact with Kafka.
   ```bash
   dotnet add package Confluent.Kafka.Core
   ```

2. **Confluent.Kafka.Core.Abstractions**  
   **Note:** This package does not need to be installed directly, as it will be included with the other packages. It provides the interfaces and abstractions used throughout the Kafka client libraries.

3. **Confluent.Kafka.Core.Idempotency.Redis**  
   For implementing idempotency using Redis as the backing store. This helps ensure that messages are processed only once.
   ```bash
   dotnet add package Confluent.Kafka.Core.Idempotency.Redis
   ```

4. **Confluent.Kafka.Core.Retry.Polly**  
   For managing blocking retries using the Polly library. This package helps implement resilience strategies in message processing.
   ```bash
   dotnet add package Confluent.Kafka.Core.Retry.Polly
   ```

5. **Confluent.Kafka.Core.OpenTelemetry**  
   For integrating with OpenTelemetry, enabling tracing and monitoring of Kafka operations in distributed systems.
   ```bash
   dotnet add package Confluent.Kafka.Core.OpenTelemetry
   ```

6. **Confluent.Kafka.Core.Serialization.JsonCore**  
   For integrating with the System.Text.Json (JsonCore) library for message serialization and deserialization. This is useful for working with JSON data in Kafka.
   ```bash
   dotnet add package Confluent.Kafka.Core.Serialization.JsonCore
   ```

7. **Confluent.Kafka.Core.Serialization.NewtonsoftJson**  
   For integrating with Newtonsoft.Json (Json.NET) for message serialization and deserialization. This package offers flexibility for working with complex JSON structures.
   ```bash
   dotnet add package Confluent.Kafka.Core.Serialization.NewtonsoftJson
   ```

8. **Confluent.Kafka.Core.Serialization.ProtobufNet**  
   For integrating with Protobuf-net for message serialization and deserialization. This package is ideal for efficient binary serialization.
   ```bash
   dotnet add package Confluent.Kafka.Core.Serialization.ProtobufNet
   ```

9. **Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro**  
   For integrating with Avro serialization and schema management through a schema registry. This enables efficient management of Avro schemas in Kafka.
   ```bash
   dotnet add package Confluent.Kafka.Core.Serialization.SchemaRegistry.Avro
   ```

10. **Confluent.Kafka.Core.Serialization.SchemaRegistry.Json**  
    For integrating with JSON serialization and schema management through a schema registry. This enables efficient management of JSON schemas in Kafka.
    ```bash
    dotnet add package Confluent.Kafka.Core.Serialization.SchemaRegistry.Json
    ```

11. **Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf**  
    For integrating with Protobuf serialization and schema management through a schema registry. This enables efficient management of Protobuf schemas in Kafka.
    ```bash
    dotnet add package Confluent.Kafka.Core.Serialization.SchemaRegistry.Protobuf
    ```

### Additional Notes
- Ensure you have the .NET SDK installed on your machine to run these commands.
- You can also install these packages using the NuGet Package Manager in Visual Studio by searching for each package name.
- After installation, you can start integrating these packages into your Kafka-related .NET project.

| [Go Back](/README.md) |
|-----------------------| 