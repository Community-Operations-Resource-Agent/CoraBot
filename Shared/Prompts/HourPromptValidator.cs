using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace Shared.Prompts
{
    public static class HourPromptValidator
    {
        public static PromptValidator<string> Create()
        {
            return async (promptContext, cancellationToken) =>
            {
                var success = DateTimeHelpers.ParseHour(promptContext.Context.Activity.Text, out DateTime dt);
                return await Task.FromResult(success);
            };
        }
    }
}
