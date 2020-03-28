using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Shared.Prompts
{
    public class CustomConfirmPrompt : ConfirmPrompt
    {
        public CustomConfirmPrompt(string dialogId, PromptValidator<bool> validator = null, string defaultLocale = null)
            : base(dialogId, validator, defaultLocale)
        {
            this.Style = ListStyle.List;
        }
    }
}
