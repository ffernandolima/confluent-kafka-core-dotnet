﻿using Confluent.Kafka.Core.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Confluent.Kafka.Core.Producer.Internal
{
    internal static class KafkaProducerExtensions
    {
        public static void ValidateTopicSuffix(this IKafkaProducer<byte[], KafkaMetadataMessage> producer, string suffix)
        {
            if (producer is null)
            {
                throw new ArgumentNullException(nameof(producer), $"{nameof(producer)} cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(suffix))
            {
                throw new ArgumentException($"{nameof(suffix)} cannot be null or whitespace.", nameof(suffix));
            }

            var producerConfig = producer!.Options!.ProducerConfig;

            if (!string.IsNullOrWhiteSpace(producerConfig!.DefaultTopic) &&
                !producerConfig!.DefaultTopic.EndsWith(suffix))
            {
                throw new KafkaProducerConfigException(
                    new[]
                    {
                        new ValidationResult(
                            $"{nameof(producerConfig.DefaultTopic)} must end with '{suffix}' suffix.",
                            new[] { nameof(producerConfig.DefaultTopic) })
                    });
            }
        }
    }
}