using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Shared.ApiInterface;
using System.Threading.Tasks;

namespace Shared.Prompts
{
    public static class UpdateResourcePromptValidator
    {
        public static PromptValidator<FoundChoice> Create(IApiInterface api)
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

                var user = await api.GetUser(promptContext.Context);
                var resources = await api.GetResourcesForUser(user);

                // Make sure the index and name still match up in case the schema changed.
                var selectedChoice = promptContext.Recognized.Value;
                var selectedIndex = selectedChoice.Index;

                if (selectedIndex >= resources.Count)
                {
                    return await Task.FromResult(false);
                }

                return await Task.FromResult(resources[selectedIndex].Name == selectedChoice.Value);
            };
        }
    }
}
