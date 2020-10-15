using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.State
{
    public class StateAccessors
    {
        /// <summary>
        /// Gets the accessor name for the dialog context property.
        /// </summary>
        /// <value>The accessor name for the dialog state property.</value>
        /// <remarks>Accessors require a unique name.</remarks
        public static string DialogContextName { get; } = "DialogContext";

        /// <summary>
        /// Gets the accessor name for the user context property.
        /// </summary>
        /// <value>The accessor name for the user context property.</value>
        /// <remarks>Accessors require a unique name.</remarks
        public static string UserContextName { get; } = "UserContext";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for DialogContext.
        /// </summary>
        /// <value>
        /// The accessor stores the dialog context for the conversation.
        /// </value>
        public IStatePropertyAccessor<DialogState> DialogContextAccessor { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for UserContext.
        /// </summary>
        /// <value>
        /// The accessor stores the user context for the conversation.
        /// </value>
        public IStatePropertyAccessor<UserContext> UserContextAccessor { get; set; }

        /// <summary>
        /// Gets the <see cref="ConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="ConversationState"/> object.</value>
        public ConversationState ConversationState { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAccessors"/> class.
        /// Contains the state management and associated accessor objects.
        /// </summary>
        /// <param name="conversationState">The state object that stores the conversation state.</param>
        public StateAccessors(ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            DialogContextAccessor = conversationState.CreateProperty<DialogState>(DialogContextName);
            UserContextAccessor = conversationState.CreateProperty<UserContext>(UserContextName);
        }

        public async Task<UserContext> GetUserContext(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return await UserContextAccessor.GetAsync(turnContext, () =>
            { return new UserContext(); }, cancellationToken).ConfigureAwait(false);
        }

        public async Task ClearUserContext(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await UserContextAccessor.DeleteAsync(turnContext, cancellationToken);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAccessors"/> class.
        /// Contains the state management and associated accessor objects.
        /// </summary>
        public static StateAccessors Create(IConfiguration configuration = null)
        {
            IStorage storage;

            if (configuration == null || string.IsNullOrEmpty(configuration.CosmosKey()))
            {
                // Use the in-memory storage provider.
                storage = new MemoryStorage();
            }
            else
            {
                // Use the Cosmos storage provider.
                storage = new CosmosDbPartitionedStorage(
                    new CosmosDbPartitionedStorageOptions
                    {
                        CosmosDbEndpoint = configuration.CosmosEndpoint(),
                        AuthKey = configuration.CosmosKey(),
                        DatabaseId = configuration.CosmosDatabase(),
                        ContainerId = configuration.CosmosConversationsContainer(),
                    });
            }

            // Create the state management and accessors.
            return new StateAccessors(new ConversationState(storage));
        }
    }
}
