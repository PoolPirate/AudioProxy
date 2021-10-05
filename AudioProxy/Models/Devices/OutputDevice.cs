using AudioProxy.Helpers;
using AudioProxy.Services;
using System;

namespace AudioProxy.Models
{
    public class OutputDevice
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = AudioDeviceHelper.GetDefaultOutputDevice().ProductName;
        public DeviceMode SoundsMode { get; set; }
        public DeviceMode InputsMode { get; set; } 

        public OutputDevice(string name, DeviceMode effectsMode, DeviceMode inputsMode)
        {
            Id = Guid.NewGuid();
            Name = name;
            SoundsMode = effectsMode;
            InputsMode = inputsMode;
        }
        public OutputDevice()
        {
            SoundsMode = new DeviceMode();
            InputsMode = new DeviceMode();
        }
    }
}
