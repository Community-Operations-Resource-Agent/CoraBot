using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Shared.Prompts
{
    public class CustomChoicePrompt : ChoicePrompt
    {
        public CustomChoicePrompt(string dialogId, PromptValidator<FoundChoice> validator = null, string defaultLocale = null)
            : base(dialogId, validator, defaultLocale)
        {
            this.Style = ListStyle.List;
        }
    }
}
