using Microsoft.Bot.Builder.Dialogs;
using Shared.Models.Helpers;
using System.Threading.Tasks;

namespace Shared.Prompts
{
    public static class DaysPromptValidator
    {
        public static PromptValidator<string> Create()
        {
            return async (promptContext, cancellationToken) =>
            {
                var success = DayFlagsHelpers.FromString(promptContext.Context.Activity.Text, ",", out DayFlags dayFlags);
                return await Task.FromResult(success);
            };
        }
    }
}
