using CSS.Challenge.Domain.Configuration;
using CSS.Challenge.Domain.Interfaces;
using CSS.Challenge.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CSS.Challenge.Domain
{
    public static class Startup
    {
        public static IServiceProvider InitServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            
            ConfigureServices(serviceCollection);
            ConfigureDefaultApplicationSettings(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static ApplicationSettings GetAppSettings()
        {
            var configuration = InitializeConfigurationSettings();

            var applicationSettingsSection = configuration.GetSection("ApplicationSettings");
            var applicationSettingsInstance = new ApplicationSettings();

            applicationSettingsSection.Bind(applicationSettingsInstance);

            return applicationSettingsInstance;
        }


        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IProcessOrdersService, ProcessOrdersService>();
            serviceCollection.AddScoped<IReceiveOrderService, ReceiveOrderService>();
            serviceCollection.AddScoped<IRandomOrderGeneratorService, RandomOrderGeneratorService>();
            serviceCollection.AddScoped<ISubmitOrderResultsService, SubmitOrderResultsService>();
        }

        private static void ConfigureDefaultApplicationSettings(IServiceCollection serviceCollection)
        {
            var configuration = InitializeConfigurationSettings();
            
            // Bind and register ApplicationSettings as singleton
            var applicationSettingsSection = configuration.GetSection("ApplicationSettings");
            var apiProcessingOptionsSection = configuration.GetSection("ApiProcessingOptions");
            var applicationSettingsInstance = new ApplicationSettings();
            var apiProcessingOptionsInstance = new ApiProcessingOptions();

            applicationSettingsSection.Bind(applicationSettingsInstance);
            apiProcessingOptionsSection.Bind(apiProcessingOptionsInstance);

            serviceCollection.AddSingleton(applicationSettingsInstance);
            serviceCollection.AddSingleton(apiProcessingOptionsInstance);
        }

        private static IConfigurationRoot InitializeConfigurationSettings()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
