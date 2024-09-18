namespace Confluent.Kafka.Core.Shared.Tests.Stubs
{
    public class TestSubject : ITestSubject
    {
        public string SomeProperty { get; set; }

        public TestSubject()
        { }

        public TestSubject(string someProperty)
        {
            SomeProperty = someProperty;
        }
    }
}
