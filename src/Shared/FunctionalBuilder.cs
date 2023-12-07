using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

    internal abstract class FunctionalBuilder<TSubject, TSubjectAbs, TSelf> : IFunctionalBuilder<TSubject, TSubjectAbs, TSelf>
        where TSubject : class, TSubjectAbs, new()
        where TSubjectAbs : class
        where TSelf : FunctionalBuilder<TSubject, TSubjectAbs, TSelf>
    {
        private readonly TSubject _seedSubject;
        private readonly Func<TSubject> _defaultSubjectFactory = () => new();
        private readonly List<Func<TSubject, TSubject>> _functions = new();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TSubject _builtSubject;

        public FunctionalBuilder(TSubjectAbs seedSubject = null)
        {
            _seedSubject = (TSubject)seedSubject;
        }

        protected virtual TSubject CreateSubject() => _defaultSubjectFactory.Invoke();

        protected virtual TSelf AppendAction(Action<TSubject> action)
        {
            if (action is not null)
            {
                _functions.Add(subject =>
                {
                    action.Invoke(subject);
                    return subject;
                });
            }

            return this as TSelf;
        }

        public virtual TSelf Clear()
        {
            ClearInternal();

            return this as TSelf;
        }

        public virtual TSubjectAbs Build()
        {
            _builtSubject ??= _functions.Aggregate(
                _seedSubject ?? CreateSubject() ?? _defaultSubjectFactory.Invoke(),
                (subject, function) => function.Invoke(subject));

            return _builtSubject;
        }

        private void ClearInternal()
        {
            _functions.Clear();
            _builtSubject = null;
        }

        #region IDisposable Members

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ClearInternal();
                }

                _disposed = true;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}