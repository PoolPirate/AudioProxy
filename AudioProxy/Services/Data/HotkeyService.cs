using AudioProxy.Models;
using AudioProxy.Options;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioProxy.Services
{
    public sealed class HotkeyService : Service
    {
        [Inject] private readonly HotkeyOptions HotkeyOptions;

        public event Func<Task> OnHotkeysChanged;

        public Keys[] GetHotkeyKeys(Hotkey hotkey)
        {
            lock (HotkeyOptions)
            {
                return HotkeyOptions.TryGetValue(hotkey, out var keys)
                    ? keys
                    : Array.Empty<Keys>();
            }
        }

        public void UpdateHotkey(Hotkey hotkey, Keys[] keys)
        {
            lock (HotkeyOptions)
            {
                if (!HotkeyOptions.TryAdd(hotkey, keys))
                {
                    HotkeyOptions[hotkey] = keys;
                }
                _ = OnHotkeysChanged?.Invoke();
            }
        }

        public bool TryGetPressedHotkey(Keys[] pressedKeys, out Hotkey? hotkey)
        {
            lock (HotkeyOptions)
            {
                foreach (var value in HotkeyOptions)
                {
                    if (pressedKeys.Length != value.Value.Length ||
                        !pressedKeys.All(x => value.Value.Contains(x)))
                    {
                        continue;
                    }

                    hotkey = value.Key;
                    return true;
                }
            }

            hotkey = null;
            return false;
        }
    }
}
