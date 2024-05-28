﻿using System.Collections.Generic;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal sealed class PropagationContext : IPropagationContext
    {
        public ActivityContext ActivityContext { get; }
        public IEnumerable<KeyValuePair<string, string>> Baggage { get; } = EmptyBaggage();

        public PropagationContext(ActivityContext activityContext, IEnumerable<KeyValuePair<string, string>> baggage)
        {
            ActivityContext = activityContext;
            Baggage = baggage ?? EmptyBaggage();
        }

        private static IEnumerable<KeyValuePair<string, string>> EmptyBaggage() => [];
    }
}
