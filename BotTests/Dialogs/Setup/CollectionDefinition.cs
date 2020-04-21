using Xunit;

namespace BotTests.Setup
{
    [CollectionDefinition("TestCollection")]
    public class TestCollectionDefinition : ICollectionFixture<TestFixture>
    {
        // Nothing needed here. This container enables defining interfaces that
        // will be applied to all test classes that are members of this collection.
    }
}
