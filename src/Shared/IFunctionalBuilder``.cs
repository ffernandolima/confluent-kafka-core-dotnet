namespace Confluent.Kafka.Core.Internal
{
    internal interface IFunctionalBuilder<out TSubject, out TSelf> : IFunctionalBuilder<TSubject, TSubject, TSelf>
        where TSubject : class, new()
        where TSelf : IFunctionalBuilder<TSubject, TSelf>
    { }
}