using Microsoft.Bot.Builder.Adapters;
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
            this.Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                .Build();

            this.Api = new CosmosInterface(Configuration);
            this.Translator = new Translator(this.Configuration);
        }

        public async Task InitializeAsync()
        {
            await this.Api.Init();
        }

        public async Task DisposeAsync()
        {
            await this.Api.Destroy();
        }
    }
}
