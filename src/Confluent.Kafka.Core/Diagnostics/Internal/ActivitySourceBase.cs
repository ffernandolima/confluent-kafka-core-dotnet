﻿using System;
using System.Diagnostics;

namespace Confluent.Kafka.Core.Diagnostics.Internal
{
    internal abstract class ActivitySourceBase
    {
        protected ActivitySource ActivitySource { get; }
        protected string ActivitySourceName => ActivitySource?.Name;
        protected string ActivitySourceVersion => ActivitySource?.Version;

        protected ActivitySourceBase(string activitySourceName, string activitySourceVersion = null)
        {
            if (string.IsNullOrWhiteSpace(activitySourceName))
            {
                throw new ArgumentException($"{nameof(activitySourceName)} cannot be null or whitespace.", nameof(activitySourceName));
            }

            ActivitySource = new ActivitySource(activitySourceName, activitySourceVersion);
        }

        public Activity StartActivity(string activityName, ActivityKind activityKind, PropagationContext context)
        {
            var activity = context is null
                ? ActivitySource.StartActivity(activityName, activityKind)
                : ActivitySource.StartActivity(activityName, activityKind, context.ActivityContext)
                                .SetBaggageItems(context.Baggage);

            return activity;
        }
    }
}