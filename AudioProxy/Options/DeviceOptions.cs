using AudioProxy.Models;
using Common.Configuration;
using System.Collections.Generic;

namespace AudioProxy.Options
{
    [SectionName("Devices")]
    public sealed class DeviceOptions : Option
    {
        public List<OutputDevice> Output { get; set; } = new List<OutputDevice>();
        public List<InputDevice> Input { get; set; } = new List<InputDevice>();
    }
}
