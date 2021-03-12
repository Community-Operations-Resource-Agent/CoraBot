using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Remy.State;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Shared.ApiInterface;
using Shared.Middleware;
using Shared.Translation;
using Microsoft.Bot.Builder.LanguageGeneration;

namespace Remy
{
    public class Startup
    {
        private IConfiguration configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            this.configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        // AddSingleton - Same for every object and every request.
        // AddScoped - Same within a request but different across different requests.
        // AddTransient - Always different. New instance for every controller and service.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add the configuration.
            services.AddSingleton(this.configuration);

            // Add the API interface.
            // Cosmos recommends a singleton for the lifetime of the application.
            // Other types may need to be scoped to the request (like Entity Framework).
            var api = new CosmosInterface(this.configuration);
            services.AddSingleton(api);

            // Add the state accessors.
            var state = StateAccessors.Create(this.configuration);
            services.AddSingleton(state);

            // Add the translator.
            var translator = new Translator(this.configuration);
            services.AddSingleton(translator);

            // Configure Language Generation files
            Dictionary<string, string> lgFilesPerLocale = new Dictionary<string, string>() {
                    {"", Path.Combine(".", "Responses", "AllResponses.lg")},
                };
            services.AddSingleton(new MultiLanguageLG(lgFilesPerLocale));

            // Configure the bot.
            services.AddBot<TheBot>(options =>
            {
                // Load the configuration settings.
                options.CredentialProvider = new SimpleCredentialProvider(
                   this.configuration.MicrosoftAppId(),
                   this.configuration.MicrosoftAppPassword());

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    Debug.WriteLine(exception.Message);

                    await context.TraceActivityAsync("Exception", exception);

                    if (!configuration.IsProduction())
                    {
                        await context.SendActivityAsync(exception.Message);
                        await context.SendActivityAsync(exception.StackTrace);
                    }

                    await context.SendActivityAsync(Shared.Phrases.Exceptions.Error);
                };

                // Auto-save the state after each turn.
                // This should be the first middleware called in order to catch state changes by any other middleware or the bot.
                options.Middleware.Add(new AutoSaveStateMiddleware(state.ConversationState));

                options.Middleware.Add(new InitApiMiddleware(api));
                options.Middleware.Add(new TypingMiddleware());
                options.Middleware.Add(new TrimIncomingMessageMiddleware());
                options.Middleware.Add(new CreateUserMiddleware(api));
                options.Middleware.Add(new TranslationMiddleware(api, translator));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseBotFramework();
        }
    }
}
