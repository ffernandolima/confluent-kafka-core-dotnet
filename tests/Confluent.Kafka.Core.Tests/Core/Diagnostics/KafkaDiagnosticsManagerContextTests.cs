using Confluent.Kafka.Core.Diagnostics;
using Confluent.Kafka.Core.Diagnostics.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Core.Diagnostics
{
    public class KafkaDiagnosticsManagerContextTests : IDisposable
    {
        private readonly ActivityListener _listener;
        private readonly Dictionary<string, string> _carrier;
        private readonly IKafkaDiagnosticsManager _diagnosticsManager;

        public KafkaDiagnosticsManagerContextTests()
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
        public void InjectContext_ShouldPropagateContextToCarrier()
        {
            // Arrange
            var activityName = "producer-activity";
            var activity = _diagnosticsManager.StartProducerActivity(activityName, _carrier);

            // Act
            _diagnosticsManager.InjectContext(activity, _carrier);

            // Assert
            Assert.NotEmpty(_carrier);
        }

        [Fact]
        public void ExtractContext_ShouldExtractValidContextFromCarrier()
        {
            // Arrange
            var activityName = "producer-activity";
            var activity = _diagnosticsManager.StartProducerActivity(activityName, _carrier);
            _diagnosticsManager.InjectContext(activity, _carrier);

            // Act
            var extractedContext = _diagnosticsManager.ExtractContext(_carrier);

            // Assert
            Assert.NotNull(extractedContext);
            Assert.Equal(activity.Context, extractedContext.ActivityContext);
        }
    }
}
