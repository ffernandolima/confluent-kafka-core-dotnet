using System;
using System.Linq.Expressions;

namespace Confluent.Kafka.Core.Specifications
{
    public abstract class ExpressionSpecification<T> : ISpecification<T>
    {
        private readonly Func<T, bool> _predicate;

        protected Expression<Func<T, bool>> Expression { get; }

        protected ExpressionSpecification(Expression<Func<T, bool>> expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression), $"{nameof(expression)} cannot be null.");
            }

            _predicate = expression.Compile();
            Expression = expression;
        }

        protected virtual bool Evaluate(T source) => _predicate.Invoke(source);

        public bool IsSatisfiedBy(T source) => Evaluate(source);
    }
}
