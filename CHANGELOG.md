# Changelog

## 2.0.0

### Breaking: telemetry attributes renamed

The diagnostics were pinned to OpenTelemetry semantic conventions v1.23.1 and now follow **v1.44.0**.
Eight attribute keys had been deprecated upstream. **Dashboards, alerts and saved queries built on the
old names will silently stop matching** — nothing fails at build or run time.

| Before (v1.23.1) | After (v1.44.0) |
|---|---|
| `messaging.client_id` | `messaging.client.id` |
| `messaging.operation` | `messaging.operation.type`, plus the new `messaging.operation.name` |
| `messaging.kafka.consumer.group` | `messaging.consumer.group.name` |
| `messaging.kafka.destination.partition` | `messaging.destination.partition.id` (now a string) |
| `messaging.kafka.message.offset` | `messaging.kafka.offset` |
| `exception.type`, `exception.message`, `exception.stacktrace` | `error.type`, plus the exception as a span event |
| `network.transport` | removed — no longer part of the Kafka attribute set |
| — | `server.port` added |

Unchanged: `messaging.system`, `messaging.destination.name`, `messaging.message.id`,
`messaging.message.body.size`, `messaging.kafka.message.key`, `messaging.kafka.message.tombstone`,
`server.address`, and the library-specific `messaging.kafka.result.*` / `messaging.kafka.processing.is_error`.

### Breaking: span names, kinds and values

- **Span names are now `{operation} {topic}`**, not `{topic} {operation}`: `publish orders`, not `orders publish`.
- **A pull-based receive is now `ActivityKind.Client`**, not `Consumer`. Only push-based processing stays `Consumer`.
- **`messaging.operation.type` uses the conventions' closed set**, so producing reports `send`, not `publish`.
  `publish` survives as the `messaging.operation.name` value.
- **`server.address` reports the configured bootstrap host**, for example `localhost`, rather than the result
  of a reverse DNS lookup. It also describes a single server rather than every bootstrap entry joined together.
- Attributes that the conventions mark conditionally required are no longer emitted unconditionally, so
  `messaging.kafka.message.tombstone` and `messaging.kafka.result.is_error` appear only when true.

### Breaking: dependency floors

Three of these cross a major version, so consumers pinned to the older majors must upgrade to take this release.

| Package | Before | After |
|---|---|---|
| `StackExchange.Redis` | 2.8.41 | **3.1.13** |
| `System.Text.Json` | 9.0.6 | **10.0.11** |
| `Microsoft.Extensions.*` | 9.0.6 | **10.0.11** |
| `Confluent.Kafka`, `Confluent.SchemaRegistry.Serdes.*` | 2.11.0 | 2.15.0 |
| `protobuf-net`, `protobuf-net.Reflection` | 3.2.52 | 3.3.8 |
| `OpenTelemetry.Api` | 1.12.0 | 1.17.0 |
| `Newtonsoft.Json` | 13.0.3 | 13.0.4 |
| `Polly` | 8.6.1 | 8.7.0 |

The `OpenTelemetry.Api` bump also clears CVE-2026-40894, which made `dotnet restore` fail under
`TreatWarningsAsErrors`.

### Breaking: public API

- Members were added to `IClientConfig`, `IConsumerConfig`, `IClientConfigBuilder`, `IConsumerConfigBuilder`,
  `IConfigurationOptionsBuilder`, `ISchemaRegistryConfigBuilder`, `IJsonSerializerConfigBuilder`,
  `IJsonDeserializerConfigBuilder`, `IJsonSerializerOptionsBuilder` and `IKafkaDiagnosticsManager`. This is a
  compile break for anyone implementing those interfaces; calling them is unaffected.
- `IConfigurationOptionsBuilder.WithSocketManager` is now `[Obsolete]`. StackExchange.Redis 3.x no longer uses
  `SocketManager`, so the setting has no effect.
- XML documentation files are no longer produced or packaged. They contained no documented members.

### Added

- `net10.0` target framework, alongside `netstandard2.0`, `netstandard2.1`, `net8.0` and `net9.0`.
- Configuration surfaced from the upgraded packages: `SaslOauthbearerMetadataAuthenticationType`,
  `SaslOauthbearerSubClaimName`, `MaxPollRecords`, `ShareAcknowledgementMode`, `MaxConnectionsPerServer`,
  `BearerAuthTokenEndpointQuery`, `ValidateBeforeDomainRules`, `AllowDuplicateProperties`,
  `RequestBufferPool`, `ResponseBufferPool`, `SentinelUser`, `SentinelPassword`, `TcpKeepAlive` and
  `HighIntegrity`.
- `IKafkaDiagnosticsManager.StartConsumerActivity` overload accepting an `ActivityKind`.

`CircuitBreaker` and `RetryPolicy` are deliberately not surfaced: StackExchange.Redis marks them
`[Experimental("SER007")]`, and wrapping them would republish an unstable API without its diagnostic.

### Fixed

- **A blocking DNS lookup ran on every span.** `KafkaServersInfo` used the `GetOrAdd` value overload, so the
  cache never prevented the lookup.
- **`messaging.client.id` ignored the configured `ClientId`** and always reported librdkafka's `rdkafka`
  default. The configured value is now used, falling back to `rdkafka` when unset, which is what the broker sees.
- **`server.address` emitted every bootstrap host joined by commas** into an attribute specified as a single
  server address.
- **Message keys of type `byte[]` were recorded as the string `System.Byte[]`.** They are now decoded.
