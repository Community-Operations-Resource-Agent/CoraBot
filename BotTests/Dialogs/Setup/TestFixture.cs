using Microsoft.Extensions.Configuration;
using Shared.ApiInterface;
using Shared.Translation;
using System.Threading.Tasks;
using Xunit;

namespace BotTests.Setup
{
    public class TestFixture : IAsyncLifetime
    {
        public IApiInterface Api { get; private set; }

        public IConfiguration Configuration { get; private set; }

        public Translator Translator { get; private set; }

        public TestFixture()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                .Build();

            Api = new CosmosInterface(Configuration);
            Translator = new Translator(Configuration);
        }

        public async Task InitializeAsync()
        {
            await Api.Init();
        }

        public async Task DisposeAsync()
        {
            await Api.Destroy();
        }
    }
}
