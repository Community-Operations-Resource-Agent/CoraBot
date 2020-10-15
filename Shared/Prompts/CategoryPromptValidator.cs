using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Prompts
{
    public static class CategoryPromptValidator
    {
        public static PromptValidator<FoundChoice> Create()
        {
            return async (promptContext, cancellationToken) =>
            {
                if (!promptContext.Recognized.Succeeded)
                {
                    return await Task.FromResult(false);
                }

                if (promptContext.Recognized.Value.Value == Phrases.None)
                {
                    return await Task.FromResult(true);
                }

                var schema = Helpers.GetSchema();
                var resource = schema.Categories.FirstOrDefault(r => r.Name == promptContext.Recognized.Value.Value);
                return await Task.FromResult(resource != null);
            };
        }
    }
}
