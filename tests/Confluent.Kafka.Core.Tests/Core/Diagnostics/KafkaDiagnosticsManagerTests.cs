using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Core.Diagnostics
{
    public sealed class KafkaDiagnosticsManagerTests : IDisposable
    {
        private readonly ActivityListener _listener;
        private readonly Dictionary<string, string> _carrier;
        private readonly IKafkaDiagnosticsManager _diagnosticsManager;

        public KafkaDiagnosticsManagerTests()
        {
            var options = new KafkaEnrichmentOptions();
            _diagnosticsManager = new KafkaDiagnosticsManager(options);
            _carrier = [];
            _listener = KafkaActivityListener.StartListening();
        }

        public void Dispose()
        {
            _listener?.Dispose();

            GC.SuppressFinalize(this);
        }

        [Fact]
        public void StartProducerActivity_ShouldStartProducerActivity()
        {
            // Arrange
            var activityName = "producer-activity";

            // Act
            var activity = _diagnosticsManager.StartProducerActivity(activityName, _carrier);

            // Assert
            Assert.NotNull(activity);
            Assert.Equal(activityName, activity.DisplayName);
            Assert.Equal(ActivityKind.Producer, activity.Kind);
        }

        [Fact]
        public void StartConsumerActivity_ShouldStartConsumerActivity()
        {
            // Arrange
            var activityName = "consumer-activity";

            // Act
            var activity = _diagnosticsManager.StartConsumerActivity(activityName, _carrier);

            // Assert
            Assert.NotNull(activity);
            Assert.Equal(activityName, activity.DisplayName);
            Assert.Equal(ActivityKind.Consumer, activity.Kind);
        }
    }
}
