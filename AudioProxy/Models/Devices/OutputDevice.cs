using System;
using AudioProxy.Helpers;

namespace AudioProxy.Models
{
    public class OutputDevice
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = AudioDeviceHelper.GetDefaultOutputDevice().ProductName;
        public DeviceMode SoundsMode { get; set; } = new DeviceMode();
        public DeviceMode InputsMode { get; set; } = new DeviceMode();

        public OutputDevice(string name, DeviceMode effectsMode, DeviceMode inputsMode)
        {
            Id = Guid.NewGuid();
            Name = name;
            SoundsMode = effectsMode;
            InputsMode = inputsMode;
        }
        public OutputDevice()
        {
        }
    }
}
