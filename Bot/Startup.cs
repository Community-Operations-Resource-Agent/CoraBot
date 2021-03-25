using Bot.Middleware;
using Bot.State;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;
using Shared.ApiInterface;
using Shared.Translation;
using System.Diagnostics;

namespace Bot
{
    public class Startup
    {
        private readonly IConfiguration Configuration;

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        // AddSingleton - Same for every object and every request.
        // AddScoped - Same within a request but different across different requests.
        // AddTransient - Always different. New instance for every controller and service.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add the configuration.
            services.AddSingleton(Configuration);

            // Add the API interface.
            // Cosmos recommends a singleton for the lifetime of the application.
            // Other types may need to be scoped to the request (like Entity Framework).
            var api = new CosmosInterface(Configuration);
            services.AddSingleton(api);

            // Add the state accessors.
            var state = StateAccessors.Create(Configuration);
            services.AddSingleton(state);

            // Add the translator.
            var translator = new Translator(Configuration);
            services.AddSingleton(translator);

            // Configure the bot.
            _ = services.AddBot<TheBot>(options =>
              {
                // Load the configuration settings.
                options.CredentialProvider = new SimpleCredentialProvider(
                     Configuration.MicrosoftAppId(),
                     Configuration.MicrosoftAppPassword());

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                  {
                      Debug.WriteLine(exception.Message);

                      await context.TraceActivityAsync("Exception", exception);

                      if (!Configuration.IsProduction())
                      {
                          await context.SendActivityAsync(exception.Message);
                          await context.SendActivityAsync(exception.StackTrace);
                      }
                      else
                      {
                        // Production only
                        await Helpers.SendIssueEmail(Configuration.GetSection("SENDGRID_API_KEY").Value, exception);
                      }

                      await context.SendActivityAsync(Phrases.Exceptions.Error);
                  };

                // Auto-save the state after each turn.
                // This should be the first middleware called in order to catch state changes by any other middleware or the bot.
                options.Middleware.Add(new AutoSaveStateMiddleware(state.ConversationState));

                  options.Middleware.Add(new InitApiMiddleware(api));
                  options.Middleware.Add(new TypingMiddleware());
                  options.Middleware.Add(new TrimIncomingMessageMiddleware());
                  options.Middleware.Add(new CreateUserMiddleware(api));
                  options.Middleware.Add(new TranslationMiddleware(api, state, translator));
              });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseBotFramework();
        }
    }
}
