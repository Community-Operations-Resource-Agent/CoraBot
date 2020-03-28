using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace Shared.Prompts
{
    public static class LessThanOrEqualPromptValidator
    {
        public static PromptValidator<string> Create()
        {
            return async (promptContext, cancellationToken) =>
            {
                var validations = (LessThanOrEqualPromptValidations)promptContext.Options.Validations;

                if (!int.TryParse(promptContext.Recognized.Value, out var inputValue))
                {
                    return await Task.FromResult(false);
                }

                var success = inputValue <= validations.Max;
                return await Task.FromResult(success);
            };
        }
    }

    public struct LessThanOrEqualPromptValidations
    {
        public int Max { get; set; }
    }
}
