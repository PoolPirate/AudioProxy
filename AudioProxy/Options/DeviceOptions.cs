using System.Collections.Generic;
using AudioProxy.Models;
using Common.Configuration;

namespace AudioProxy.Options
{
    [SectionName("Devices")]
    public sealed class DeviceOptions : Option
    {
        public List<OutputDevice> Output { get; set; } = new List<OutputDevice>();
        public List<InputDevice> Input { get; set; } = new List<InputDevice>();
    }
}
