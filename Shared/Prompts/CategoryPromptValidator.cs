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

                var schema = Helpers.GetSchema();
                var category = schema.Categories.FirstOrDefault(c => c.Name == promptContext.Recognized.Value.Value);
                return await Task.FromResult(category != null);
            };
        }
    }
}
