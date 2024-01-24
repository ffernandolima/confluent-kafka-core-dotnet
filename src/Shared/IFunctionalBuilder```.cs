using System;

namespace Confluent.Kafka.Core.Internal
{
    internal interface IFunctionalBuilder<out TSubject, out TSubjectAbs, out TSelf> : IDisposable
        where TSubject : class, TSubjectAbs, new()
        where TSelf : IFunctionalBuilder<TSubject, TSubjectAbs, TSelf>
    {
        TSelf Clear();
        TSubjectAbs Build();
    }
}