using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Confluent.Kafka.Core.Internal
{
    internal abstract class FunctionalBuilder<TSubject, TSubjectAbs, TSelf> :
        IFunctionalBuilder<TSubject, TSubjectAbs, TSelf>
            where TSubject : class, TSubjectAbs
            where TSubjectAbs : class
            where TSelf : FunctionalBuilder<TSubject, TSubjectAbs, TSelf>
    {
        private readonly TSubject _seedSubject;
        private readonly IConfiguration _configuration;
        private readonly Func<TSubject> _defaultFactory;
        private readonly Dictionary<int, object> _parameters = [];
        private readonly List<Func<TSubject, TSubject>> _functions = [];

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TSubject _builtSubject;

        public FunctionalBuilder(
            TSubject seedSubject = null,
            IConfiguration configuration = null)
        {
            _seedSubject = seedSubject;
            _configuration = configuration;
            _defaultFactory = () => _parameters.Count > 0
                ? (TSubject)Activator.CreateInstance(
                    typeof(TSubject),
                    _parameters.OrderBy(x => x.Key).Select(x => x.Value).ToArray())
                : Activator.CreateInstance<TSubject>();
        }

        public FunctionalBuilder(
            TSubjectAbs seedSubjectAbs = null,
            IConfiguration configuration = null)
            : this((TSubject)seedSubjectAbs, configuration)
        { }

        protected virtual TSubject CreateSubject()
            => _defaultFactory.Invoke();

        protected virtual TSelf AppendParameter(Action<IDictionary<int, object>> action)
        {
            action?.Invoke(_parameters);

            return this as TSelf;
        }

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

        protected virtual IConfigurationSection GetSection(string sectionKey)
            => _configuration?.GetSection(sectionKey);

        protected virtual TSubject Bind(
            TSubject subject,
            string sectionKey,
            Action<BinderOptions> configureOptions = null)
        {
            var configurationSection = GetSection(sectionKey);

            configurationSection.Bind(
                subject,
                configureOptions ??= options =>
                {
                    options.BindNonPublicProperties = false;
                    options.ErrorOnUnknownConfiguration = false;
                });

            return subject;
        }

        public virtual TSelf Clear()
        {
            CheckDisposed();
            ClearInternal();

            return this as TSelf;
        }

        public virtual TSubjectAbs Build()
        {
            CheckDisposed();

            _builtSubject ??= _functions.Aggregate(
                _seedSubject ?? CreateSubject() ?? _defaultFactory.Invoke(),
                (subject, function) => function.Invoke(subject));

            return _builtSubject;
        }

        private void ClearInternal()
        {
            _functions?.Clear();
            _parameters?.Clear();
            _builtSubject = null;
        }

        private void CheckDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            var builderType = GetType();

            throw new ObjectDisposedException(builderType.ExtractTypeName());
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