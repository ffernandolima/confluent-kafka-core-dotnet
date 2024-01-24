namespace Confluent.Kafka.Core.Internal
{
    internal abstract class FunctionalBuilder<TSubject, TSelf> : FunctionalBuilder<TSubject, TSubject, TSelf>, IFunctionalBuilder<TSubject, TSelf>
        where TSubject : class, new()
        where TSelf : FunctionalBuilder<TSubject, TSelf>
    {
        public FunctionalBuilder(TSubject seedSubject = null)
            : base(seedSubject)
        { }
    }
}