using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Prompts
{
    public static class ResourcePromptValidator
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

                var validations = (ResourcePromptValidations)promptContext.Options.Validations;

                var schema = Helpers.GetSchema();
                var category = schema.Categories.FirstOrDefault(c => c.Name == validations.Category);
                if (category == null)
                {
                    return await Task.FromResult(false);
                }

                var resource = category.Resources.FirstOrDefault(r => r.Name == promptContext.Recognized.Value.Value);
                return await Task.FromResult(resource != null);
            };
        }
    }

    public struct ResourcePromptValidations
    {
        public string Category { get; set; }
    }
}
