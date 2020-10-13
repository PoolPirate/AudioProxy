using System;
using System.IO;

namespace AudioProxy.Helpers
{
    public static class ConfigPathHelper
    {
        public static string GetConfigFolderPath()
            => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AudioProxy");

        public static string GetAppsettingsPath()
            => Path.Combine("appsettings.json");

        public static string GetDevicesConfigPath()
            => Path.Combine(GetConfigFolderPath(), "devices.yaml");

        public static string GetProfilesConfigPath()
            => Path.Combine(GetConfigFolderPath(), "profiles.yaml");

        public static string GetSoundsConfigPath()
            => Path.Combine(GetConfigFolderPath(), "sounds.yaml");

        public static string GetGeneralConfigPath()
            => Path.Combine(GetConfigFolderPath(), "config.yaml");
    }
}
