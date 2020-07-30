using System;
using System.Threading;
using System.Threading.Tasks;
using Greyshirt.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Shared.ApiInterface;

namespace Greyshirt.Dialogs
{
    public abstract class DialogBase
    {
        protected readonly StateAccessors state;
        protected readonly DialogSet dialogs;
        protected readonly IApiInterface api;
        protected readonly IConfiguration configuration;
        protected readonly MultiLanguageLG lgGenerator;

        public DialogBase(StateAccessors state, DialogSet dialogs, IApiInterface api, IConfiguration configuration, MultiLanguageLG lgGenerator)
        {
            this.state = state;
            this.dialogs = dialogs;
            this.api = api;
            this.configuration = configuration;
            this.lgGenerator = lgGenerator;
        }

        public abstract Task<WaterfallDialog> GetWaterfallDialog(ITurnContext turnContext, CancellationToken cancellationToken);

        /// <summary>
        /// JIT creates the dialog if necessary and begins the dialog.
        /// Type <typeparamref name="T"/> is the type of dialog, deriving from <see cref="DialogBase"/>
        /// </summary>
        public async Task<DialogTurnResult> BeginDialogAsync(DialogContext dialogContext, string dialogId, object options, CancellationToken cancellationToken)
        {
            // Only create the dialog if it doesn't exist.
            if (dialogs.Find(dialogId) == null)
            {
                var dialog = CreateFromDialogId(dialogId);
                if (dialog != null)
                {
                    var waterfallDialog = await dialog.GetWaterfallDialog(dialogContext.Context, cancellationToken);
                    this.dialogs.Add(waterfallDialog);
                }
            }

            return await dialogContext.BeginDialogAsync(dialogId, options, cancellationToken);
        }

        /// <summary>
        /// JIT creates the dialog stack if necessary and continues the dialog.
        /// Type <typeparamref name="T"/> is the type of dialog, deriving from <see cref="DialogBase"/>
        /// </summary>
        public async Task<DialogTurnResult> ContinueDialogAsync(DialogContext dialogContext, CancellationToken cancellationToken)
        {
            // Go through each entry in the context stack.
            foreach (var entry in dialogContext.Stack)
            {
                // Only create the dialog if it doesn't exist.
                if (this.dialogs.Find(entry.Id) == null)
                {
                    var dialog = CreateFromDialogId(entry.Id);
                    if (dialog != null)
                    {
                        var waterfallDialog = await dialog.GetWaterfallDialog(dialogContext.Context, cancellationToken);
                        this.dialogs.Add(waterfallDialog);
                    }
                }
            }

            return await dialogContext.ContinueDialogAsync(cancellationToken);
        }

        private DialogBase CreateFromDialogId(string dialogId)
        {
            // Get the class type. The Id of each dialog is the class name.
            Type type = Type.GetType(dialogId);
            if (type != null && type.IsSubclassOf(typeof(DialogBase)))
            {
                // Create an instance of the dialog and add it to the dialog set.
                return (DialogBase)Activator.CreateInstance(type, this.state, this.dialogs, this.api, this.configuration, this.lgGenerator);
            }

            return null;
        }
    }
}
