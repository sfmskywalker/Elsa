﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Activities.Telnyx.Activities;
using Elsa.Activities.Telnyx.ActivityTypes;
using Elsa.Activities.Telnyx.Bookmarks;
using Elsa.Activities.Telnyx.Client.Services;
using Elsa.Activities.Telnyx.Options;
using Elsa.Activities.Telnyx.Services;
using Elsa.Activities.Telnyx.Webhooks.Consumers;
using Elsa.Activities.Telnyx.Webhooks.Events;
using Elsa.Activities.Telnyx.Webhooks.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NodaTime;
using NodaTime.Serialization.JsonNet;
using Refit;

namespace Elsa.Activities.Telnyx.Extensions
{
    public static class SetupExtensions
    {
        public static ElsaOptions AddTelnyx(this ElsaOptions elsaOptions, Action<TelnyxOptions>? configure = default, Func<IServiceProvider, HttpClient>? httpClientFactory = default)
        {
            var services = elsaOptions.Services;

            // Configure Telnyx.
            var telnyxOptions = services.GetTelnyxOptions();
            configure?.Invoke(telnyxOptions);

            // Activities.
            elsaOptions
                .AddActivitiesFrom<AnswerCall>()
                .AddConsumer<TriggerWorkflows, TelnyxWebhookReceived>();

            // Services.
            services
                .AddActivityTypeProvider<NotificationActivityTypeProvider>()
                .AddBookmarkProvider<NotificationBookmarkProvider>()
                .AddNotificationHandlers(typeof(TriggerWorkflows))
                .AddScoped<IWebhookHandler, WebhookHandler>()
                .AddScoped(telnyxOptions.ExtensionProviderFactory);

            // Telnyx API Client.
            var refitSettings = CreateRefitSettings();
            
            services
                .AddApiClient<ICallsApi>(refitSettings, httpClientFactory)
                .AddTransient<ITelnyxClient, TelnyxClient>();

            return elsaOptions;
        }

        public static IEndpointConventionBuilder MapTelnyxWebhook(this IEndpointRouteBuilder endpoints, string routePattern = "telnyx-hook")
        {
            return endpoints.MapPost(routePattern, HandleTelnyxRequest);
        }

        public static IServiceCollection AddApiClient<T>(this IServiceCollection services, RefitSettings refitSettings, Func<IServiceProvider, HttpClient>? httpClientFactory) where T : class
        {
            if (httpClientFactory == null)
            {
                services.AddRefitClient<T>(refitSettings).ConfigureHttpClient((sp, client) =>
                {
                    var options = sp.GetRequiredService<TelnyxOptions>();
                    client.BaseAddress = options.ApiUrl;
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
                }).AddHttpMessageHandler(() => new Spy());
            }
            else
            {
                services.AddScoped(sp =>
                {
                    var httpClient = httpClientFactory(sp);
                    var options = sp.GetRequiredService<TelnyxOptions>();
                    httpClient.BaseAddress ??= options.ApiUrl;
                    httpClient.DefaultRequestHeaders.Authorization ??= new AuthenticationHeaderValue("Bearer", options.ApiKey);

                    return RestService.For<T>(httpClient, refitSettings);
                });
            }

            return services;
        }

        public static RefitSettings CreateRefitSettings()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            serializerSettings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            return new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(serializerSettings)
            };
        }

        private static TelnyxOptions GetTelnyxOptions(this IServiceCollection services)
        {
            var telnyxOptions = (TelnyxOptions?) services.FirstOrDefault(x => x.ServiceType == typeof(TelnyxOptions))?.ImplementationInstance;

            if (telnyxOptions == null)
            {
                telnyxOptions = new TelnyxOptions();
                services.AddSingleton(telnyxOptions);
            }

            return telnyxOptions;
        }

        private static async Task HandleTelnyxRequest(HttpContext context)
        {
            var services = context.RequestServices;
            var webhookHandler = services.GetRequiredService<IWebhookHandler>();
            await webhookHandler.HandleAsync(context);
        }
        
    }

    class Spy : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var json = await (request.Content as StringContent)?.ReadAsStringAsync(cancellationToken)!;
            return await base.SendAsync(request, cancellationToken);
        }
    }

}