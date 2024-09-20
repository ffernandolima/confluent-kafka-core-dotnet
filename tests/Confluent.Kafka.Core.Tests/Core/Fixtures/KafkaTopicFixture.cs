using Confluent.Kafka.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Confluent.Kafka.Core.Tests.Core.Fixtures
{
    public sealed class KafkaTopicFixture : IAsyncLifetime
    {
        private readonly int _numPartitions;
        private readonly short _replicationFactor;
        private readonly string _bootstrapServers;
        private readonly IEnumerable<string> _topics;

        private readonly IAdminClient _adminClient;

        public KafkaTopicFixture(string bootstrapServers, IEnumerable<string> topics, int numPartitions = 1, short replicationFactor = 1)
        {
            _numPartitions = numPartitions;
            _replicationFactor = replicationFactor;
            _bootstrapServers = bootstrapServers;
            _topics = topics;

            var clientConfig = new AdminClientConfig
            {
                BootstrapServers = _bootstrapServers
            };

            _adminClient = new AdminClientBuilder(clientConfig).Build();
        }

        public async Task InitializeAsync()
        {
            await DeleteTopicsAsync();
            await CreateTopicsAsync();
        }

        public async Task DisposeAsync()
        {
            await DeleteTopicsAsync();

            _adminClient.Dispose();
        }

        private async Task CreateTopicsAsync()
        {
            try
            {
                var specifications = new List<TopicSpecification>();

                specifications.AddRange(
                    Enumerable.Range(0, _topics.Count())
                              .Select(idx => new TopicSpecification
                              {
                                  Name = _topics.ElementAt(idx),
                                  NumPartitions = _numPartitions,
                                  ReplicationFactor = _replicationFactor
                              }));

                await _adminClient.CreateTopicsAsync(specifications);
            }
            catch (CreateTopicsException ex)
            {
                if (ex.Results.All(report => report.Error.Code != ErrorCode.TopicAlreadyExists))
                {
                    throw;
                }
            }
        }

        private async Task DeleteTopicsAsync()
        {
            try
            {
                await _adminClient.DeleteTopicsAsync(_topics);
            }
            catch (DeleteTopicsException ex)
            {
                if (ex.Results.All(report => report.Error.Code != ErrorCode.UnknownTopicOrPart))
                {
                    throw;
                }
            }
        }
    }
}
