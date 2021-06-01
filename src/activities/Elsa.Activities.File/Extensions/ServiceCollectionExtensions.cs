using Elsa;
using Elsa.Activities.File;
using Elsa.Activities.File.Bookmarks;
using Elsa.Activities.File.Options;
using Elsa.Activities.File.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileActivities(this IServiceCollection services)
        {
            return services;
        }

        public static ElsaOptionsBuilder AddFileActivities(this ElsaOptionsBuilder builder, Action<FileWatcherOptions> configureOptions = null) 
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            builder.Services.AddFileServices(configureOptions);

            builder.AddActivity<DeleteFile>()
                .AddActivity<FileExists>()
                .AddActivity<OutFile>()
                .AddActivity<ReadFile>()
                .AddActivity<TempFile>()
                .AddActivity<WatchDirectory>();

            return builder;
        }

        public static IServiceCollection AddFileServices(this IServiceCollection services, Action<FileWatcherOptions> configureOptions = null)
        {
            if (configureOptions != null)
                services.Configure<FileWatcherOptions>(configureOptions);

            services.AddHostedService<FileWatcherService>();
            services.AddBookmarkProvider<FileSystemChangedBookmarkProvider>();

            return services;
        }
    }
}