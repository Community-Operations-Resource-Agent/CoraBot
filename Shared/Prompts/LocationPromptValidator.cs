using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Shared.Prompts
{
    public static class LocationPromptValidator
    {
        private static HttpClient httpClient = new HttpClient();

        public static PromptValidator<string> Create(IConfiguration configuration)
        {
            return async (promptContext, cancellationToken) =>
            {
                var location = await Helpers.StringToLocation(configuration, promptContext.Context.Activity.Text);
                return await Task.FromResult(location != null);
            };
        }
    }
}
