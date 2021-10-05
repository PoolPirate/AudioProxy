using AudioProxy.Models;
using AudioProxy.Options;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AudioProxy.Services
{
    [InitializationPriority(1)]
    public sealed class DeviceService : Service
    {
        [Inject] private readonly DeviceOptions DeviceOptions;
        [Inject] private readonly AudioOutputService AudioOutputService;
        [Inject] private readonly AudioInputService AudioInputService;

        public event Func<Task> OnDevicesChanged;

        public IEnumerable<OutputDevice> GetAllOutputDevices()
        {
            lock (DeviceOptions.Output)
            {
                return DeviceOptions.Output.ToArray();
            }
        }
        public IEnumerable<InputDevice> GetAllInputDevices()
        {
            lock (DeviceOptions.Input)
            {
                return DeviceOptions.Input.ToArray();
            }
        }

        public void CreateOutputDevice(string id, DeviceMode effectsMode, DeviceMode inputsMode)
        {
            var outputDevice = new OutputDevice(id, effectsMode, inputsMode);
            lock (DeviceOptions.Output)
            {
                DeviceOptions.Output.Add(outputDevice);
                AudioOutputService.AddDevice(outputDevice);
            }
            _ = OnDevicesChanged?.Invoke();
        }
        public void CreateInputDevice(string id, DeviceMode mode)
        {
            var inputDevice = new InputDevice(id, mode);
            lock (DeviceOptions.Input)
            {
                DeviceOptions.Input.Add(inputDevice);
                AudioInputService.AddDevice(inputDevice);
            }
            _ = OnDevicesChanged?.Invoke();
        }

        public void UpdateOutputDevice(OutputDevice outputDevice, Action<OutputDevice> changeDelegate)
        {
            lock (DeviceOptions.Output)
            {
                changeDelegate.Invoke(outputDevice);
            }
            _ = OnDevicesChanged?.Invoke();
        }
        public void UpdateInputDevice(InputDevice inputDevice, Action<InputDevice> changeDelegate)
        {
            lock (DeviceOptions.Input)
            {
                changeDelegate.Invoke(inputDevice);
            }
            _ = OnDevicesChanged?.Invoke();
        }

        public void DeleteOutputDevice(OutputDevice outputDevice)
        {
            lock (DeviceOptions.Output)
            {
                DeviceOptions.Output.Remove(outputDevice);
                AudioOutputService.RemoveDevice(outputDevice);
            }
            _ = OnDevicesChanged?.Invoke();
        }
        public void DeleteInputDevice(InputDevice inputDevice)
        {
            lock (DeviceOptions.Input)
            {
                DeviceOptions.Input.Remove(inputDevice);
                AudioInputService.RemoveDevice(inputDevice);
            }
            _ = OnDevicesChanged?.Invoke();
        }
    }
}
