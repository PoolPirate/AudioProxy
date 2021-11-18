using System;
using System.IO;

namespace AudioProxy.Helpers
{
    public static class PathHelper
    {
        public static string AudioProxyRoot { get; }
            = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AudioProxy");

        public static string ConfigRoot { get; }
            = Path.Combine(AudioProxyRoot, "Config");

        public static string GetAppsettingsPath()
            => Path.Combine("appsettings.json");

        public static string GetDevicesConfigPath()
            => Path.Combine(ConfigRoot, "devices.yaml");

        public static string GetProfilesConfigPath()
            => Path.Combine(ConfigRoot, "profiles.yaml");

        public static string GetSoundsConfigPath()
            => Path.Combine(ConfigRoot, "sounds.yaml");

        public static string GetGeneralConfigPath()
            => Path.Combine(ConfigRoot, "general.yaml");

        public static string GetDefaultYoutubeCacheLocation()
            => Path.Combine(AudioProxyRoot, "Cache", "Youtube");
    }
}
