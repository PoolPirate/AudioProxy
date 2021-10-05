using AudioProxy.Helpers;
using AudioProxy.Services;
using System;

namespace AudioProxy.Models
{
    public class InputDevice
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = AudioDeviceHelper.GetDefaultInputDevice().ProductName;
        public DeviceMode Mode { get; set; }

        public InputDevice(string name, DeviceMode mode)
        {
            Id = Guid.NewGuid();
            Name = name;
            Mode = mode;
        }
        public InputDevice()
        {
            Mode = new DeviceMode();
        }
    }
}
