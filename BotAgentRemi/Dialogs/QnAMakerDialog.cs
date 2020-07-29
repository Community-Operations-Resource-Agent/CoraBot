using BotAgentRemi.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Shared;
using Shared.ApiInterface;
using Shared.QnAMaker;
using Shared.Translation;
using Shared.Prompts;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace BotAgentRemi.Dialogs
{
    public class QnAMakerDialog : DialogBase
    {
        public static string Name = typeof(QnAMakerDialog).FullName;

        Translator translator;
        QnAMaker qnaMaker;

        public QnAMakerDialog(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
            : base (state, dialogs, api, configuration, lgGenerator)
        {
            this.translator = new Translator(configuration);
            this.qnaMaker = new QnAMaker(configuration);
        }

        public override Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellation)
        {
            return Task.Run(() =>
            {
                return new WaterfallDialog(Name, new WaterfallStep[]
                {
                    async (dialogContext, cancellationToken) =>
                    {
                        return await dialogContext.PromptAsync(
                            Prompt.TextPrompt,
                            new PromptOptions { Prompt = Shared.Phrases.QnAMaker.GetQuestionFromUser},
                            cancellationToken);
                    },
                    async (dialogContext, cancellationToken) =>
                    {
                        string question = (string)dialogContext.Result;
                        string answer = await qnaMaker.GetAnswerFromQnAMaker(question, cancellationToken);

                        await Messages.SendAsync(answer, dialogContext.Context, cancellationToken);
                        return await dialogContext.EndDialogAsync(false, cancellationToken);
                    }
                });
            });
        }
    }
}
