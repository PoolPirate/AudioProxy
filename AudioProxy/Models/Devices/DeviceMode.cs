using System;
using System.Linq;

namespace AudioProxy.Models
{
    public sealed class DeviceMode
    {
        public TransferMode TransferMode { get; set; } = TransferMode.Always;

        public Keys[] Keys { get; set; } = Array.Empty<Keys>();

        public DeviceMode(TransferMode transferMode, Keys[] keys)
        {
            TransferMode = transferMode;
            Keys = keys;
        }
        public DeviceMode()
        {
        }

        public bool IsFulfilled(Keys[] pressedKeys)
            => TransferMode switch
            {
                TransferMode.Always => true,
                TransferMode.PushToTalk => Keys.Length != 0 && Keys.All(x => pressedKeys.Contains(x)),
                TransferMode.PushToMute => Keys.Length != 0 && !Keys.All(x => pressedKeys.Contains(x)),
                TransferMode.Never => false,
                _ => true,
            };
    }
}
