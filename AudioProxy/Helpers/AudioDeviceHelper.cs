using NAudio.Wave;

namespace AudioProxy.Helpers
{
    public static class AudioDeviceHelper
    {
        public static WaveOutCapabilities GetDefaultOutputDevice()
            => WaveOut.GetCapabilities(-1);
        public static WaveInCapabilities GetDefaultInputDevice()
            => WaveIn.GetCapabilities(-1);

        public static WaveOutCapabilities[] GetAllOutputDevices()
        {
            var devices = new WaveOutCapabilities[WaveOut.DeviceCount + 1];

            for (int i = 0; i < WaveOut.DeviceCount + 1; i++)
            {
                devices[i] = WaveOut.GetCapabilities(i - 1);
            }

            return devices;
        }
        public static WaveInCapabilities[] GetAllInputDevices()
        {
            var devices = new WaveInCapabilities[WaveIn.DeviceCount + 1];

            for (int i = 0; i < WaveIn.DeviceCount + 1; i++)
            {
                devices[i] = WaveIn.GetCapabilities(i - 1);
            }

            return devices;
        }

        public static int GetOutputDeviceNumberByName(string name)
        {
            var devices = GetAllOutputDevices();

            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].ProductName.Trim() == name)
                {
                    return i - 1;
                }
            }

            return -1;
        }
        public static int GetInputDeviceNumberByName(string name)
        {
            var devices = GetAllInputDevices();

            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].ProductName.Trim() == name)
                {
                    return i - 1;
                }
            }

            return -1;
        }
    }
}
