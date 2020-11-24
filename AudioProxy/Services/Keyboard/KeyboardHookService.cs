using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AudioProxy.Models;
using Common.Services;
using Microsoft.Extensions.Logging;

namespace AudioProxy.Services
{
    public sealed class KeyboardHookService : Service
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern bool GetMessage(out WindowMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        private static extern bool TranslateMessage([In] ref WindowMessage lpMsg);

        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage([In] ref WindowMessage lpMsg);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static WindowMessage Message;
        private static IntPtr HookId;

        private LowLevelKeyboardProc CallbackDelegate;

        private static void SetHook(LowLevelKeyboardProc proc)
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            HookId = SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }

        private readonly ConcurrentDictionary<Keys, bool> PressedKeys;

        public event Func<Keys, Task> OnKeyDown;
        public event Func<Keys, Task> OnKeyPressed;
        public event Func<Keys, Task> OnKeyUp;

        public KeyboardHookService()
        {
            PressedKeys = new ConcurrentDictionary<Keys, bool>();
        }

        public Keys[] GetPressedKeys()
            => PressedKeys.Where(x => x.Value)
                .Select(x => x.Key)
                .ToArray();

        protected override ValueTask RunAsync()
        {
            CallbackDelegate = new LowLevelKeyboardProc(HookCallback);
            SetHook(CallbackDelegate);

            while (!GetMessage(out Message, IntPtr.Zero, 0, 0))
            {
                TranslateMessage(ref Message);
                DispatchMessage(ref Message);
            }
            Logger.LogInformation("Message Pump Quit!");
            return default;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int iwParam = wParam.ToInt32();
                if (iwParam == WM_KEYDOWN || iwParam == WM_SYSKEYDOWN)
                {
                    short vkCode = Marshal.ReadInt16(lParam);
                    _ = Task.Run(() => OnKeyDown?.Invoke((Keys) vkCode));
                    if (PressedKeys.TryAdd((Keys)vkCode, true) || PressedKeys.TryUpdate((Keys)vkCode, true, false)) //Make sure to only trigger on the first time this is fired
                    {
                        _ = Task.Run(() => OnKeyPressed?.Invoke((Keys) vkCode));
                    }
                }
                if (iwParam == WM_KEYUP || iwParam == WM_SYSKEYUP)
                {
                    short vkCode = Marshal.ReadInt16(lParam);
                    _ = Task.Run(() => OnKeyUp?.Invoke((Keys) vkCode));
                    PressedKeys.TryUpdate((Keys) vkCode, false, true);
                }

                return IntPtr.Zero;
            }

            return CallNextHookEx(HookId, nCode, wParam, lParam);
        }
    }
}
