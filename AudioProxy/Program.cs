using AudioProxy.Helpers;
using AudioProxy.Options;
using AudioProxy.Services;
using Common.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading.Tasks;

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

            if (generalOptions.FirstLaunch)
            {
                var contextMenuService = host.Services.GetRequiredService<ContextMenuService>();

                contextMenuService.ShowNotification("You can find me down here!",
                    "Double click the Tray Icon to open AudioProxy in your Browser");
            }

            int port = config.GetValue<int>("Port");
            BrowserHelper.RunAudioProxyInBrowser(generalOptions.FirstLaunch, port);
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
                        webBuilder.UseSetting(WebHostDefaults.ServerUrlsKey, $"http://localhost:{port}");
                    });
                })
                .ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.AddJsonFile(PathHelper.GetAppsettingsPath(), false);
                    configBuilder.AddYamlFile(PathHelper.GetDevicesConfigPath(), true);
                    configBuilder.AddYamlFile(PathHelper.GetProfilesConfigPath(), true);
                    configBuilder.AddYamlFile(PathHelper.GetSoundsConfigPath(), true);
                    configBuilder.AddYamlFile(PathHelper.GetGeneralConfigPath(), true);
                })
                .ConfigureLogging((context, loggerBuilder) =>
                {
                    loggerBuilder.AddConsole();
                    loggerBuilder.AddConfiguration(context.Configuration);
                })
                .Build();
    }
}
