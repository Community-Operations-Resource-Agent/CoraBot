using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Prompts
{
    public static class KeywordPromptValidator
    {
        public static PromptValidator<string> Create()
        {
            return async (promptContext, cancellationToken) =>
            {
                var success = Phrases.Keywords.List.Any(k => string.Equals(promptContext.Recognized.Value, k, StringComparison.OrdinalIgnoreCase));
                return await Task.FromResult(success);
            };
        }
    }
}
