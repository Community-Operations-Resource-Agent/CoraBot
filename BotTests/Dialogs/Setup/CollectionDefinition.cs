using Xunit;

namespace BotTests.Setup
{
    [CollectionDefinition("TestCollection")]
    public class TestCollectionDefinition : ICollectionFixture<TestFixture>
    {
    }
}
