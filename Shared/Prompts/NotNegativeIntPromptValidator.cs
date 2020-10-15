using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace Shared.Prompts
{
    public static class NotNegativeIntPromptValidator
    {
        public static PromptValidator<int> Create()
        {
            return async (promptContext, cancellationToken) =>
            {
                if (!promptContext.Recognized.Succeeded)
                {
                    return await Task.FromResult(false);
                }

                return await Task.FromResult(promptContext.Recognized.Value >= 0);
            };
        }
    }
}
