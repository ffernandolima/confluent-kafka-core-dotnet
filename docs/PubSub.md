| [Main](/README.md) > [Usage](/docs/Usage.md) > Producers/Consumers |
|--------------------------------------------------------------------|

### Producers :factory:

Here's an example of how to use a Producer in your .NET application:

```C#
 IServiceCollection services = new ServiceCollection()
     .AddKafka(builder =>
         builder.AddKafkaProducer<Null, string>((_, builder) =>
             builder.WithProducerConfiguration(builder =>
                 builder.WithBootstrapServers("localhost:9092"))));

 using var serviceProvider = services.BuildServiceProvider();

 var producer = serviceProvider.GetRequiredService<IKafkaProducer<Null, string>>();

 // Synchronous production of a message
 producer.Produce("test-topic", new Message<Null, string> { Value = "test" });

 // Asynchronous production of a message
 await producer.ProduceAsync("test-topic", new Message<Null, string> { Value = "test" });
```

By default, producers are going to be registered as **Singleton**.

### Producer Configurations :gear:

Some configurations should be pointed out once they enable custom behaviors:

### Consumers :zap:

Here's an example of how to use a Consumer in your .NET application:

```C#
 IServiceCollection services = new ServiceCollection()
     .AddKafka(builder =>
         builder.AddKafkaConsumer<Null, string>((_, builder) =>
            builder.WithConsumerConfiguration(builder =>
                builder.WithBootstrapServers("localhost:9092")
                       .WithGroupId("test-consumer-group")
                       .WithAutoOffsetReset(AutoOffsetReset.Earliest)
                       .WithTopicSubscriptions(["test-topic"])));

 using var serviceProvider = services.BuildServiceProvider();

 var consumer = serviceProvider.GetRequiredService<IKafkaConsumer<Null, string>>();

 var consumeResult = consumer.Consume();
```

By default, consumers are going to be registered as **Singleton**.

### Consumer Configurations :gear:

Some configurations should be pointed out once they enable custom behaviors:

| [Go Back](/docs/Usage.md) |
|---------------------------| 