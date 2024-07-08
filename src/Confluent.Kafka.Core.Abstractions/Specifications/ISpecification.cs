namespace Confluent.Kafka.Core.Specifications
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T source);
    }
}
