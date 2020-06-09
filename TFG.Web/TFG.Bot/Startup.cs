using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TFG.Bot.Dialogs.Login;
using TFG.Dialogs;
using TFG.Domain.Helpers.Mappers;

namespace Microsoft.BotBuilderSamples
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            ConfigureBot(services);

            ConfigureStorage(services);

            ConfigureMappers(services);

            services.AddSingleton<TFG.Domain.Shared.Abstractions.Repositories.IEdamamRepository, TFG.Infraestructure.ExternalApi.Edamam.EdamamRepository>();
            services.AddSingleton<TFG.Domain.Shared.Abstractions.Domains.IEdamamDomain, TFG.Domain.Domains.EdamamDomain>();
            services.AddSingleton<TFG.Domain.Shared.Abstractions.Services.IEdamamService, TFG.Domain.Services.EdamamService>();

            services.AddSingleton<TFG.Domain.Shared.Abstractions.Repositories.IMessagesRepository, TFG.Infraestructure.Repository.MessagesRepository>();
            services.AddSingleton<TFG.Domain.Shared.Abstractions.Domains.IMessagesDomain, TFG.Domain.Domains.MessagesDomain>();
            services.AddSingleton<TFG.Domain.Shared.Abstractions.Services.IMessagesService, TFG.Domain.Services.MessagesService>();
        }

        private void ConfigureBot(IServiceCollection services)
        {
            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the bot services (LUIS, QnA) as a singleton.
            services.AddSingleton<IBotServices, BotServices>();

            // Create the bot as a transient.
            services.AddTransient<IBot, DialogBot<MainDialog>>();

            services.AddScoped<MainDialog>();

            services.AddScoped<LoginState>();
        }

        private void ConfigureStorage(IServiceCollection services)
        {
            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();
            // Create the User state. (Used in this bot's Dialog implementation.)
            services.AddSingleton<UserState>();
            // Create the Conversation state. (Used by the Dialog system itself.)
            services.AddSingleton<ConversationState>();
        }

        private void ConfigureMappers(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DTOEdamamMappers());
                mc.AddProfile(new DTOMessagesMappers());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
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
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
