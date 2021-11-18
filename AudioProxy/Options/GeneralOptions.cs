using AudioProxy.Helpers;
using Common.Configuration;
using System;

namespace AudioProxy.Options
{
    [SectionName("General")]
    public class GeneralOptions : Option
    {
        public bool FirstLaunch { get; set; } = true;

        public bool YoutubeCaching { get; set; } = true;

        public string YoutubeCacheLocation { get; set; } = PathHelper.GetDefaultYoutubeCacheLocation();

        public GeneralOptions()
        {
        }
    }
}
