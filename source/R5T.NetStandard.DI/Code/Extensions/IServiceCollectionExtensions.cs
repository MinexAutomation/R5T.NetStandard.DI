﻿using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace R5T.NetStandard
{
    public static class IServiceCollectionExtensions
    {
        public const string LoggingServiceAddedMessage = @"LOGGING. Adding service...";


        #region Add

        public static IServiceCollection AddTransient<TService, TImplementation>(this IServiceCollection services, ILogger logger)
            where TService : class
            where TImplementation : class, TService
        {
            logger.LogAddingTransientService<TImplementation>();

            services.AddTransient<TService, TImplementation>();

            return services;
        }

        public static IServiceCollection AddScoped<TServiceAndImplementation>(this IServiceCollection services, ILogger logger)
            where TServiceAndImplementation : class
        {
            logger.LogAddingScopedService<TServiceAndImplementation>();

            services.AddScoped<TServiceAndImplementation>();

            return services;
        }

        public static IServiceCollection AddScoped<TServiceAndImplementation>(this IServiceCollection services, Func<IServiceProvider, TServiceAndImplementation> implementationFactory, ILogger logger)
            where TServiceAndImplementation : class
        {
            logger.LogAddingScopedService<TServiceAndImplementation>();

            services.AddScoped<TServiceAndImplementation>(implementationFactory);

            return services;
        }

        public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services, ILogger logger)
            where TService : class
            where TImplementation : class, TService
        {
            logger.LogAddingScopedService<TImplementation>();

            services.AddScoped<TService, TImplementation>();

            return services;
        }

        public static IServiceCollection AddScoped<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory, ILogger logger)
            where TService : class
            where TImplementation : class, TService
        {
            logger.LogAddingScopedService<TImplementation>();

            services.AddScoped<TService, TImplementation>(implementationFactory);

            return services;
        }

        public static IServiceCollection AddScopedForward<TService, TForwardedService>(this IServiceCollection services, ILogger logger)
            where TForwardedService : TService
            where TService : class
        {
            logger.LogDebug($@"{typeof(TService)} <= {typeof(TForwardedService)} - SCOPED. Adding forwarded service...");

            services.AddScoped<TService>(serviceProvider =>
            {
                var forwardedService = serviceProvider.GetRequiredService<TForwardedService>();
                return forwardedService;
            });

            logger.LogInformation($@"{typeof(TService)} <= {typeof(TForwardedService)} - SCOPED. Added forwarded service.");

            return services;
        }

        public static IServiceCollection AddSingleton<TServiceAndImplementation>(this IServiceCollection services, ILogger logger)
            where TServiceAndImplementation : class
        {
            logger.LogAddingSingletonService<TServiceAndImplementation>();

            services.AddSingleton<TServiceAndImplementation>();

            return services;
        }

        public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services, ILogger logger)
            where TService : class
            where TImplementation : class, TService
        {
            logger.LogAddingSingletonService<TImplementation>();

            services.AddSingleton<TService, TImplementation>();

            return services;
        }

        public static IServiceCollection AddSingleton<TService, TImplementation>(this IServiceCollection services, Func<IServiceProvider, TImplementation> implementationFactory, ILogger logger)
            where TService : class
            where TImplementation : class, TService
        {
            logger.LogAddingSingletonService<TImplementation>();

            services.AddSingleton<TService, TImplementation>(implementationFactory);

            return services;
        }

        public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, TService implementationInstance, ILogger logger)
            where TService : class
        {
            logger.LogAddingSingletonService(typeof(TService));

            services.AddSingleton<TService>(implementationInstance);

            return services;
        }

        public static IServiceCollection AddSingleton<TService>(this IServiceCollection services, Func<IServiceProvider, TService> implementationFactory, ILogger logger)
            where TService : class
        {
            TService implementationTypeLoggingWrapper(IServiceProvider serviceProvider)
            {
                var output = implementationFactory(serviceProvider);

                logger.LogAddingSingletonService(output.GetType());

                return output;
            }

            services.AddSingleton<TService>(implementationTypeLoggingWrapper);

            return services;
        }

        /// <summary>
        /// Allows specifying that a different service should provide the specified service by forwarding the different service as the specified service.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="TForwardedService">The different service type that implements the service type.</typeparam>
        public static IServiceCollection AddSingletonForward<TService, TForwardedService>(this IServiceCollection services, ILogger logger)
            where TForwardedService : TService
            where TService : class
        {
            logger.LogDebug($@"{typeof(TService)} <= {typeof(TForwardedService)} - SINGLETON. Adding forwarded service...");

            services.AddSingleton<TService>(serviceProvider =>
            {
                var forwardedService = serviceProvider.GetRequiredService<TForwardedService>();
                return forwardedService;
            });

            logger.LogInformation($@"{typeof(TService)} <= {typeof(TForwardedService)} - SINGLETON. Added forwarded service.");

            return services;
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Adds the configuration instance as the implementation instance for the IConfiguration service.
        /// </summary>
        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConfiguration>(configuration);

            return services;
        }

        /// <summary>
        /// Adds the configuration instance as the implementation instance for the IConfiguration service.
        /// </summary>
        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            services.AddSingleton<IConfiguration>(configuration, logger);

            return services;
        }

        public static IConfiguration GetConfiguration(this IServiceCollection services)
        {
            var output = services.GetIntermediateRequiredService<IConfiguration>();
            return output;
        }

        /// <summary>
        /// Configures <typeparamref name="TOptions"/> using the <see cref="IConfiguration"/> added to the services collection.
        /// </summary>
        public static IServiceCollection Configure<TOptions>(this IServiceCollection services)
            where TOptions : class
        {
            var configuration = services.GetConfiguration();

            services
                .Configure<TOptions>(configuration)
                ;

            return services;
        }

        /// <summary>
        /// Allow the <typeparamref name="TOptions"/> service to be requested directly instead of using the <see cref="IOptions{TOptions}"/> interface.
        /// </summary>
        public static IServiceCollection ConfigureDirect<TOptions>(this IServiceCollection services, ILogger logger)
            where TOptions : class, new()
        {
            services
                .AddSingleton<TOptions>(serviceProviderInstance =>
                {
                    var configurationOptionsInstance = serviceProviderInstance.GetOptions<TOptions>();
                    return configurationOptionsInstance;
                }, logger)
                ;

            return services;
        }

        /// <summary>
        /// Allow the <typeparamref name="TOptions"/> service to be requested directly instead of using the <see cref="IOptions{TOptions}"/> interface.
        /// </summary>
        public static IServiceCollection ConfigureDirect<TOptions>(this IServiceCollection services)
            where TOptions : class, new()
        {
            services
                .AddSingleton<TOptions>(serviceProviderInstance =>
                {
                    var configurationOptionsInstance = serviceProviderInstance.GetOptions<TOptions>();
                    return configurationOptionsInstance;
                })
                ;

            return services;
        }

        #endregion

        public static IServiceCollection AddLogging(this IServiceCollection services, Action<ILoggingBuilder, ILogger> configure, ILogger logger)
        {
            logger.LogInformation(IServiceCollectionExtensions.LoggingServiceAddedMessage);

            services.AddLogging((loggingBuilder) =>
            {
                configure(loggingBuilder, logger);
            });

            return services;
        }

        #region Miscellaneous

        public static IServiceProvider BuildServiceProvider(this IServiceCollection services, ILogger logger)
        {
            var output = services.BuildServiceProvider();

            logger.LogBuiltServiceProvider();

            return output;
        }

        /// <summary>
        /// Build a service provider from the current state of the service collection and get a required service.
        /// </summary>
        public static T GetIntermediateRequiredService<T>(this IServiceCollection services)
        {
            var intermediateServiceProvider = services.BuildServiceProvider();

            var output = intermediateServiceProvider.GetRequiredService<T>();
            return output;
        }

        #endregion
    }
}
