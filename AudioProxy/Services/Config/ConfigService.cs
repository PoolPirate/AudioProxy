using AudioProxy.Helpers;
using AudioProxy.Options;
using Common.Configuration;
using Common.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace AudioProxy.Services
{
    public sealed class ConfigService : Service
    {
        [Inject] private readonly IHostApplicationLifetime LifeTime;
        [Inject] private readonly ProfileService ProfileService;
        [Inject] private readonly SoundService SoundService;
        [Inject] private readonly DeviceService DeviceService;
        [Inject] private readonly HotkeyService HotkeyService;

        [Inject] private readonly DeviceOptions DeviceOptions;
        [Inject] private readonly ProfileOptions ProfileOptions;
        [Inject] private readonly SoundOptions SoundOptions;
        [Inject] private readonly HotkeyOptions HotkeyOptions;
        [Inject] private readonly AudioOptions AudioOptions;
        [Inject] private readonly GeneralOptions GeneralOptions;

        private readonly Serializer YamlSerializer;

        public ConfigService()
        {
            YamlSerializer = new Serializer();
        }

        protected override ValueTask InitializeAsync()
        {
            LifeTime.ApplicationStopping.Register(() =>
            {
                TryWriteAllConfigsAsync().Wait();
            });

            DeviceService.OnDevicesChanged += () => TryWriteDevicesConfigAsync();
            ProfileService.OnProfilesChanged += () => TryWriteProfilesConfigAsync();
            SoundService.OnSoundsChanged += () => TryWriteSoundsConfigAsync();
            HotkeyService.OnHotkeysChanged += () => TryWriteGeneralConfigAsync();
            return default;
        }

        public async Task<bool> SetupConfigFilesAsync()
        {
            try
            {
                if (!Directory.Exists(PathHelper.AudioProxyRoot))
                {
                    Directory.CreateDirectory(PathHelper.AudioProxyRoot);
                }
                if (!Directory.Exists(PathHelper.ConfigRoot))
                {
                    Directory.CreateDirectory(PathHelper.ConfigRoot);
                }

                if (!File.Exists(PathHelper.GetGeneralConfigPath()))
                {
                    await TryWriteGeneralConfigAsync(true);
                }
                if (!File.Exists(PathHelper.GetDevicesConfigPath()))
                {
                    await TryWriteDevicesConfigAsync(true);
                }
                if (!File.Exists(PathHelper.GetProfilesConfigPath()))
                {
                    await TryWriteProfilesConfigAsync(true);
                }
                if (!File.Exists(PathHelper.GetSoundsConfigPath()))
                {
                    await TryWriteSoundsConfigAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing config files: {ex.StackTrace}");
                return false;
            }
        }

        public async Task TryWriteAllConfigsAsync()
        {
            await TryWriteGeneralConfigAsync();
            await TryWriteDevicesConfigAsync();
            await TryWriteProfilesConfigAsync();
            await TryWriteSoundsConfigAsync();
        }

        public async Task TryWriteGeneralConfigAsync(bool defaultConfig = false)
        {
            try
            {
                IOption hotkeyOptions = defaultConfig ? new HotkeyOptions() : HotkeyOptions;
                IOption audioOptions = defaultConfig ? new AudioOptions() : AudioOptions;
                IOption generalOptions = defaultConfig ? new GeneralOptions() : GeneralOptions;
                string yamlConfig = YamlSerializer.Serialize(new Dictionary<string, object>()
                {
                    [hotkeyOptions.SectionName] = hotkeyOptions,
                    [audioOptions.SectionName] = audioOptions,
                    [generalOptions.SectionName] = generalOptions,
                });
                await File.WriteAllTextAsync(PathHelper.GetGeneralConfigPath(), yamlConfig);
            }
            catch
            {
                Console.WriteLine("Failed writing general config!");
            }
        }
        public async Task TryWriteDevicesConfigAsync(bool defaultConfig = false)
        {
            try
            {
                IOption config = defaultConfig ? new DeviceOptions() : DeviceOptions;
                string yamlConfig = YamlSerializer.Serialize(new Dictionary<string, object>()
                {
                    [config.SectionName] = config
                });
                await File.WriteAllTextAsync(PathHelper.GetDevicesConfigPath(), yamlConfig);
            }
            catch
            {
                Console.WriteLine("Failed writing devices config!");
            }
        }
        public async Task TryWriteProfilesConfigAsync(bool defaultConfig = false)
        {
            try
            {
                IOption config = defaultConfig ? new DeviceOptions() : ProfileOptions;
                string yamlConfig = YamlSerializer.Serialize(new Dictionary<string, object>()
                {
                    [config.SectionName] = config
                });
                await File.WriteAllTextAsync(PathHelper.GetProfilesConfigPath(), yamlConfig);
            }
            catch
            {
                Console.WriteLine("Failed writing profiles config!");
            }
        }
        public async Task TryWriteSoundsConfigAsync(bool defaultConfig = false)
        {
            try
            {
                IOption config = defaultConfig ? new DeviceOptions() : SoundOptions;
                string yamlConfig = YamlSerializer.Serialize(new Dictionary<string, object>()
                {
                    [config.SectionName] = config
                });
                await File.WriteAllTextAsync(PathHelper.GetSoundsConfigPath(), yamlConfig);
            }
            catch
            {
                Console.WriteLine("Failed writing sounds config!");
            }
        }
    }
}
