using Confluent.Kafka.Core.Internal;
using Microsoft.Extensions.Configuration;

namespace Confluent.Kafka.Core.Shared.Tests.Stubs
{
    internal class TestFunctionalBuilder :
        FunctionalBuilder<TestSubject, ITestSubject, TestFunctionalBuilder>
    {
        public TestFunctionalBuilder(
            IConfiguration configuration = null,
            TestSubject seedSubject = null)
            : base(seedSubject, configuration)
        { }

        public TestFunctionalBuilder FromConfiguration(string sectionKey)
        {
            AppendAction(subject =>
            {
                if (!string.IsNullOrWhiteSpace(sectionKey))
                {
                    subject = Bind(subject, sectionKey);
                }
            });
            return this;
        }

        public TestFunctionalBuilder WithParameter(string someProperty)
        {
            AppendParameter(parameters => parameters[0] = someProperty);
            return this;
        }

        public TestFunctionalBuilder WithProperty(string someProperty)
        {
            AppendAction(subject => subject.SomeProperty = someProperty);
            return this;
        }
    }
}
