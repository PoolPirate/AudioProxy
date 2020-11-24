using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;
using AudioProxy.Helpers;
using AudioProxy.Options;
using AudioProxy.Services;
using Common.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AudioProxy
{
    public class Program
    {
        //private const string ApplicationUrl = "https://localhost:51005";

        [SuppressMessage("Style", "IDE1006")]
        public static async Task Main(string[] args)
        {
            using var host = CreateHost(args);
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            var configService = host.Services.GetRequiredService<ConfigService>();
            var config = host.Services.GetRequiredService<IConfiguration>();
            var generalOptions = host.Services.GetRequiredService<GeneralOptions>();

            if (!await configService.SetupConfigFilesAsync())
            {
                logger.LogCritical($"Could not read nor create app config files!");
                return;
            }

            var validationResult = await host.Services.ValidateOptionsAsync(Assembly.GetExecutingAssembly());
            if (!validationResult.IsSuccessful)
            {
                logger.LogCritical($"Config File Validation Failed!\nReason: {validationResult.Reason}");
                return;
            }

            await host.Services.InitializeApplicationServicesAsync(Assembly.GetExecutingAssembly());
            host.Services.RunApplicationServices(Assembly.GetExecutingAssembly());

            await host.StartAsync();

            int port = config.GetValue<int>("Port");
            var browserStartInfo = new ProcessStartInfo(generalOptions.FirstLaunch ? $"https://localhost:{port}/About" : $"https://localhost:{port}")
            {
                UseShellExecute = true
            };
            Process.Start(browserStartInfo);
            generalOptions.FirstLaunch = false;
            await configService.TryWriteGeneralConfigAsync();
            await host.WaitForShutdownAsync();
        }

        public static IHost CreateHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        int port = context.Configuration.GetValue<int>("Port");
                        webBuilder.UseSetting(WebHostDefaults.ServerUrlsKey, $"https://localhost:{port}");
                    });
                })
                .ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.AddJsonFile(ConfigPathHelper.GetAppsettingsPath(), false);
                    configBuilder.AddYamlFile(ConfigPathHelper.GetDevicesConfigPath(), true);
                    configBuilder.AddYamlFile(ConfigPathHelper.GetProfilesConfigPath(), true);
                    configBuilder.AddYamlFile(ConfigPathHelper.GetSoundsConfigPath(), true);
                    configBuilder.AddYamlFile(ConfigPathHelper.GetGeneralConfigPath(), true);
                })
                .ConfigureLogging((context, loggerBuilder) =>
                {
                    loggerBuilder.AddConsole();
                    loggerBuilder.AddConfiguration(context.Configuration);
                })
                .Build();
    }
}
